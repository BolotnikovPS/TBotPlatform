using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Statistics;

namespace TBotPlatform.Common.Contexts;

internal class TelegramContextLog(ILogger<TelegramContextLog> logger) : ITelegramContextLog
{
    public Task HandleLog(TelegramContextFullLogMessage message, CancellationToken cancellationToken)
    {
        logger.LogDebug(
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

    public Task HandleEnqueueLog(int requestCount, int elapsedSeconds, Guid operationGuid, CancellationToken cancellationToken)
    {
        logger.LogDebug("Отправлено {count} запросов в telegram за {second} миллисекунд. OperationGuid: {operationGuid}", requestCount, elapsedSeconds, operationGuid.ToString());

        return Task.CompletedTask;
    }
}
