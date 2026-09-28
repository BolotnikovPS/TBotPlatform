#nullable enable

using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Handlers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Handlers;

/// <summary>
/// Обработчик обновлений для механизма получения апдейтов из Telegram.Bot
/// (<see cref="TelegramBotClientExtensions.ReceiveAsync(ITelegramBotClient, IUpdateHandler, ReceiverOptions?, CancellationToken)"/>).
/// Передаёт обновления в <see cref="ITelegramUpdateProcessor"/> и типизированно логирует ошибки получения
/// (в том числе <see cref="ApiRequestException.ErrorCode"/> и <see cref="ApiRequestException.Parameters"/>).
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
            // 400/403 — постоянные ошибки (некорректный запрос, бот заблокирован): повтор не поможет.
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
                // Ожидаемое завершение при остановке сервиса.
                break;

            default:
                logger.LogError(exception, "Ошибка при получении обновлений бота {bot} (источник {source})", botName, source);
                break;
        }

        return Task.CompletedTask;
    }
}
