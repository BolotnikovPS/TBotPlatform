using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.BackgroundServices;

internal class TelegramContextHostedService(ILogger<TelegramContextHostedService> logger, BotsDataCollection botsDataCollection, IServiceProvider services) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var taskList = new List<Task>();

        foreach (var bot in botsDataCollection)
        {
            var task = Task.Factory.StartNew(async () =>
            {
                var telegramContext = services.GetRequiredKeyedService<ITelegramContext>(bot);

                var settings = telegramContext.GetTelegramSettings();

                var result = await telegramContext.GetMe(stoppingToken);

                logger.LogDebug("Запущен бот {name}", result.FirstName);

                var offset = 0;

                var updateType = settings.UpdateType.IsNotNull()
                    ? settings?.UpdateType?.ToList()
                    : null;

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var updates = await telegramContext.GetUpdates(offset, allowedUpdates: updateType, cancellationToken: stoppingToken);

                        if (updates.IsNull())
                        {
                            await Task.Delay(settings.HostWaitMilliSecond, stoppingToken);
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
                                var scopedStartReceivingHandler = scope.ServiceProvider.GetRequiredKeyedService<IStartReceivingHandler>(settings.BotName);

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
                                    throw new("Не удалось обработать сообщение с telegram");
                                }

                                await scopedStartReceivingHandler.HandleUpdate(settings.BotName, update, markupNextState, telegramMessageUserData, stoppingToken);
                            }
                            catch (Exception ex)
                            {
                                exception = ex;
                            }
                            finally
                            {
                                timer.Stop();

                                sbLog.AppendLine($"Время выполнения: {timer.Elapsed}");

                                var logLevel = exception.IsNotNull()
                                    ? LogLevel.Error
                                    : LogLevel.Debug;

                                logger.Log(logLevel, exception, "Ошибка обработки входящего сообщения {updateId} {log}", update.Id, sbLog.ToString());

                                offset = update.Id + 1;
                            }
                        }

                        await Task.Delay(settings.HostWaitMilliSecond, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Неожиданное исключение при обработке обновлений");
                    }
                }

            },
            stoppingToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Current
            );

            taskList.Add(task);
        }

        await Task.WhenAll(taskList);
    }
}