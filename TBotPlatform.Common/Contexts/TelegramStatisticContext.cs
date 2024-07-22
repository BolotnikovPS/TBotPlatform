using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts;

internal class TelegramStatisticContext(ILogger<TelegramStatisticContext> logger) : ITelegramStatisticContext
{
    public Task HandleStatisticAsync(StatisticMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Отправка сообщения: {message}", message.ToJson());

        return Task.CompletedTask;
    }

    public Task HandleErrorStatisticAsync(StatisticMessage message, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Ошибка отправки сообщения: {message}", message.ToJson());

        return Task.CompletedTask;
    }
}