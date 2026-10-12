#nullable enable

using System.Diagnostics;
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
    private int _consecutivePollingErrors;
    private long _lastPollingErrorTimestamp;

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

        if (source is not HandleErrorSource.PollingError || cancellationToken.IsCancellationRequested)
        {
            return Task.CompletedTask; ;
        }

        // ReceiveAsync keeps polling immediately after HandleErrorAsync returns, so a network outage would
        // otherwise become a hot loop (one failed HTTP request + Error log per pass). Apply an exponential
        // backoff with full jitter here; the delay is awaited by ReceiveAsync before the next GetUpdates.
        return DelayForPollingBackoff(cancellationToken);
    }

    private Task DelayForPollingBackoff(CancellationToken cancellationToken)
    {
        var now = Stopwatch.GetTimestamp();
        var last = Interlocked.Exchange(ref _lastPollingErrorTimestamp, now);

        var attempt = last != 0
                      && Stopwatch.GetElapsedTime(last, now) <= Backoff.ResetThreshold
            ? Interlocked.Increment(ref _consecutivePollingErrors)
            : (_consecutivePollingErrors = 1);

        var delay = Backoff.GetDelay(attempt);

        logger.LogDebug(
            "Пауза {seconds} с. перед следующей попыткой получения обновлений бота {bot} (последовательная ошибка {attempt})",
            delay.TotalSeconds,
            botName,
            attempt);

        return Task.Delay(delay, cancellationToken);
    }
}
