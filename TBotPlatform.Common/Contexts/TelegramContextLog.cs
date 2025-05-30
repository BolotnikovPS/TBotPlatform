﻿using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts;

internal class TelegramContextLog(ILogger<TelegramContextLog> logger) : ITelegramContextLog
{
    public Task HandleLog(TelegramContextFullLogMessage message, CancellationToken cancellationToken)
    {
        logger.LogDebug("Отправка сообщения: {message}", message.ToJson());

        return Task.CompletedTask;
    }

    public Task HandleErrorLog(TelegramContextFullLogMessage message, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Ошибка отправки сообщения: {message}", message.ToJson());

        return Task.CompletedTask;
    }

    public Task HandleEnqueueLog(int requestCount, int elapsedSeconds, Guid operationGuid, CancellationToken cancellationToken)
    {
        logger.LogDebug("Отправлено {count} запросов в telegram за {second} миллисекунд. OperationGuid: {operationGuid}", requestCount, elapsedSeconds, operationGuid.ToString());

        return Task.CompletedTask;
    }
}