using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts;

internal class TelegramContextLog(ILogger<TelegramContextLog> logger) : ITelegramContextLog
{
    public Task HandleLogAsync(TelegramContextLogMessage message, CancellationToken cancellationToken)
    {
        logger.LogDebug("Отправка сообщения: {message}", message.ToJson());

        return Task.CompletedTask;
    }

    public Task HandleErrorLogAsync(TelegramContextLogMessage message, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Ошибка отправки сообщения: {message}", message.ToJson());

        return Task.CompletedTask;
    }
}