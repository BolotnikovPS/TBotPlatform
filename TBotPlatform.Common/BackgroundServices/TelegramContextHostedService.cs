using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.BackgroundServices;

internal class TelegramContextHostedService(
    ILogger<TelegramContextHostedService> logger,
    TelegramSettings settings,
    ITelegramContext telegramContext,
    IServiceProvider services,
    ITelegramUpdateHandler telegramUpdateHandler
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var result = await telegramContext.MakeRequestAsync(request => request.GetMeAsync(stoppingToken), stoppingToken);

        logger.LogDebug("Запущен бот {name}", result.FirstName);

        var offset = 0;

        var updateType = settings.UpdateType.IsNotNull()
            ? settings?.UpdateType?.Select(MapChatUpdate)
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
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IStartReceivingHandler>();

                        MarkupNextState markupNextState = null;

                        if (update.Type == UpdateType.CallbackQuery)
                        {
                            var data = update.CallbackQuery?.Data;

                            if (data.IsNotNull() && data!.TryParseJson<MarkupNextState>(out var newMarkupNextState))
                            {
                                markupNextState = newMarkupNextState;
                            }
                        }

                        var telegramMessageUserData = telegramUpdateHandler.GetTelegramMessageUserData(update);
                        var chatUpdate = await telegramUpdateHandler.GetChatUpdateAsync(telegramMessageUserData, update, stoppingToken);
                        await scopedProcessingService.HandleUpdateAsync(chatUpdate, markupNextState, telegramMessageUserData, stoppingToken);
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
    }

    private static UpdateType MapChatUpdate(EChatUpdateType updateTypes)
        => updateTypes switch
        {
            EChatUpdateType.Message => UpdateType.Message,
            EChatUpdateType.EditedMessage => UpdateType.EditedMessage,
            EChatUpdateType.InlineQuery => UpdateType.InlineQuery,
            EChatUpdateType.ChosenInlineResult => UpdateType.ChosenInlineResult,
            EChatUpdateType.CallbackQuery => UpdateType.CallbackQuery,
            EChatUpdateType.ChannelPost => UpdateType.ChannelPost,
            EChatUpdateType.EditedChannelPost => UpdateType.EditedChannelPost,
            EChatUpdateType.ShippingQuery => UpdateType.ShippingQuery,
            EChatUpdateType.PreCheckoutQuery => UpdateType.PreCheckoutQuery,
            EChatUpdateType.Poll => UpdateType.Poll,
            EChatUpdateType.PollAnswer => UpdateType.PollAnswer,
            EChatUpdateType.MyChatMember => UpdateType.MyChatMember,
            EChatUpdateType.ChatMember => UpdateType.ChatMember,
            EChatUpdateType.ChatJoinRequest => UpdateType.ChatJoinRequest,
            _ => UpdateType.Unknown,
        };
}