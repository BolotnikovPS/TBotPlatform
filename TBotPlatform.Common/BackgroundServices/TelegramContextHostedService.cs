﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.BackgroundServices;

internal class TelegramContextHostedService(
    ILogger<TelegramContextHostedService> logger,
    TelegramContextHostedServiceSettings settings,
    ITelegramContext telegramContext,
    IServiceProvider services
    ) : BackgroundService
{
    private const int WaitMilliSecond = 1 * 1000;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var result = await telegramContext.MakeRequestAsync(request => request.GetMeAsync(stoppingToken), stoppingToken);

        logger.LogDebug("Запущен бот {name}", result.FirstName);

        var offset = 0;

        var updateType = settings.UpdateType.IsNotNull()
            ? settings.UpdateType
            : null;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var updates = await telegramContext.MakeRequestAsync(
                    request => request.GetUpdatesAsync(offset, allowedUpdates: updateType, cancellationToken: stoppingToken),
                    stoppingToken
                    );

                if (updates.IsNull())
                {
                    await Task.Delay(WaitMilliSecond, stoppingToken);
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
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IStartReceivingHandler>();

                        MarkupNextState markupNextState = null;

                        if (update.Type == UpdateType.CallbackQuery)
                        {
                            var data = update.CallbackQuery?.Data;

                            if (data.IsNotNull())
                            {
                                markupNextState = JsonConvert.DeserializeObject<MarkupNextState>(data!);
                            }
                        }

                        await scopedProcessingService.HandleUpdateAsync(update, markupNextState, stoppingToken);
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

                await Task.Delay(WaitMilliSecond, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Неожиданное исключение при обработке обновлений");
            }
        }
    }
}