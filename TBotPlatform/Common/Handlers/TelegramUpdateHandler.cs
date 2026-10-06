#nullable enable

using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Handlers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Handlers;

/// <summary>
/// Update handler for the Telegram.Bot update-receiving mechanism
/// (<see cref="TelegramBotClientExtensions.ReceiveAsync(ITelegramBotClient, IUpdateHandler, ReceiverOptions?, CancellationToken)"/>).
/// Passes updates to <see cref="ITelegramUpdateProcessor"/> and logs errors during receiving
/// (including <see cref="ApiRequestException.ErrorCode"/> and <see cref="ApiRequestException.Parameters"/>).
/// </summary>
internal sealed class TelegramUpdateHandler(
    ITelegramUpdateProcessor updateProcessor,
    string botName,
    ILogger logger
    ) : IUpdateHandler
{
    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        => updateProcessor.ProcessUpdate(botName, update, cancellationToken);

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        switch (exception)
        {
            // 400/403 — permanent errors (invalid request, bot is blocked): retry won't help.
            case ApiRequestException apiException when apiException.ErrorCode is 400 or 403:
                logger.LogWarning(
                    "Ошибка Telegram API {errorCode} при получении обновлений бота {bot} (источник {source}): {message}",
                    apiException.ErrorCode,
                    botName,
                    source,
                    apiException.Message
                    );
                break;

            case ApiRequestException apiException:
                logger.LogError(
                    exception,
                    "Ошибка Telegram API {errorCode} при получении обновлений бота {bot} (источник {source}, retryAfter {retryAfter}, migrateToChatId {migrateToChatId})",
                    apiException.ErrorCode,
                    botName,
                    source,
                    apiException.Parameters?.RetryAfter,
                    apiException.Parameters?.MigrateToChatId
                    );
                break;

            case OperationCanceledException when cancellationToken.IsCancellationRequested:
                // Expected shutdown when the service stops.
                break;

            default:
                logger.LogError(exception, "Ошибка при получении обновлений бота {bot} (источник {source})", botName, source);
                break;
        }

        return Task.CompletedTask;
    }
}
