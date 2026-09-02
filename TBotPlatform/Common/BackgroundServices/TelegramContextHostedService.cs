using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using TBotPlatform.Common.BackgroundServices.Base;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.BackgroundServices;

internal class TelegramContextHostedService(ILogger<TelegramContextHostedService> logger, List<string> bots, IServiceProvider services) : BackgroundServiceBase<TelegramContextHostedService>
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var taskList = bots.Select(q =>
        {
            return Task.Factory.StartNew(() =>
            {
                return ExecuteBot(q, stoppingToken);
            },
            stoppingToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Current
            ).Unwrap();
        });

        await Task.WhenAll(taskList);
    }

    private async Task ExecuteBot(string bot, CancellationToken cancellationToken)
    {
        var telegramContext = services.GetRequiredKeyedService<ITelegramContext>(bot);

        var settings = telegramContext.GetBotSetting();

        var result = await telegramContext.GetMe(cancellationToken);

        logger.LogDebug("Запущен бот {name}", result.FirstName);

        var offset = 0;

        var updateType = settings.UpdatePolicy.IsNotNull()
            ? settings.UpdatePolicy.Type?.ToList()
            : null;

        var limit = settings.UpdatePolicy.IsNotNull()
            ? settings.UpdatePolicy?.Capacity
            : null;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var updates = await telegramContext.GetUpdates(offset, limit, allowedUpdates: updateType, cancellationToken: cancellationToken);

                if (updates.IsNull())
                {
                    await Task.Delay(settings.HostWaitMilliSecond, cancellationToken);
                    continue;
                }

                foreach (var update in updates)
                {
                    Exception exception = null;
                    var sbLog = new StringBuilder();

                    var timer = Stopwatch.StartNew();

                    try
                    {
                        sbLog.AppendLine($"Поступило сообщение: {update.ToJson()}");

                        await using var scope = services.CreateAsyncScope();
                        var scopedStartReceivingHandler = scope.ServiceProvider.GetRequiredKeyedService<IStartReceivingHandler>(settings.BotName) ?? throw new("Обработчик сообщений отсутствует.");

                        MarkupNextState markupNextState = null;

                        if (update.Type == UpdateType.CallbackQuery)
                        {
                            var data = update.CallbackQuery?.Data;

                            if (data.IsNotNull() && data!.TryParseJson<MarkupNextState>(out var newMarkupNextState))
                            {
                                markupNextState = newMarkupNextState;
                            }
                        }

                        if (!update.TryGetMessageUserData(out var telegramMessageUserData))
                        {
                            throw new("Не удалось обработать входящий запрос с telegram");
                        }

                        var resultReceivingHandler = await scopedStartReceivingHandler.HandleUpdate(settings.BotName, update, markupNextState, telegramMessageUserData, cancellationToken);

                        if (!resultReceivingHandler.IsSuccess)
                        {
                            sbLog.AppendLine("Не удалось обработать данные запроса с telegram");
                            throw new(resultReceivingHandler.Error.ToJson());
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    finally
                    {
                        timer.Stop();

                        sbLog.AppendLine($"Время выполнения: {timer.Elapsed}");

                        var logLevel = exception.IsNotNull() ? LogLevel.Error : LogLevel.Debug;

                        logger.Log(logLevel, exception, "Результат обработки входящего сообщения {updateId}: {log}", update.Id, sbLog.ToString());

                        offset = update.Id + 1;
                    }
                }

                await Task.Delay(settings.HostWaitMilliSecond, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Неожиданное исключение при обработке обновлений");
            }
        }
    }
}
