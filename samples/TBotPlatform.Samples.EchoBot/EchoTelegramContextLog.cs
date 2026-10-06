using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Statistics;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Example of a custom Telegram interaction log.
/// Connect it instead of the standard one: <c>AddTelegramContext&lt;EchoTelegramContextLog&gt;()</c>.
/// </summary>
internal sealed class EchoTelegramContextLog(ILogger<EchoTelegramContextLog> logger) : ITelegramContextLog
{
    public Task HandleLog(TelegramContextFullLogMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Telegram {operationType} chat {chatId} operation {operationGuid}",
            message.Request?.OperationType,
            message.Request?.ChatId,
            message.Request?.OperationGuid);

        return Task.CompletedTask;
    }

    public Task HandleErrorLog(TelegramContextFullLogMessage message, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Ошибка Telegram {operationType} chat {chatId} operation {operationGuid}",
            message.Request?.OperationType,
            message.Request?.ChatId,
            message.Request?.OperationGuid);

        return Task.CompletedTask;
    }

    public Task HandleEnqueueLog(int requestCount, int elapsedMilliseconds, Guid operationGuid, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Отправлено {count} запросов в telegram за {milliseconds} мс. OperationGuid: {operationGuid}",
            requestCount,
            elapsedMilliseconds,
            operationGuid);

        return Task.CompletedTask;
    }
}
