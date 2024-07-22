using TBotPlatform.Contracts.Statistics;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface ITelegramStatisticContext
{
    Task HandleStatisticAsync(StatisticMessage message, CancellationToken cancellationToken);
    Task HandleErrorStatisticAsync(StatisticMessage message, CancellationToken cancellationToken);
}