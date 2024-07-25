using TBotPlatform.Contracts.Statistics;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    /// <summary>
    /// Исполнение метода в очереди с сохранением статистики
    /// </summary>
    /// <param name="method"></param>
    /// <param name="statisticMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ExecuteEnqueueSafety(Task method, StatisticMessage statisticMessage, CancellationToken cancellationToken)
    {
        try
        {
            await Enqueue(() => method, cancellationToken);
            await telegramStatisticContext.HandleStatisticAsync(statisticMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            await telegramStatisticContext.HandleErrorStatisticAsync(statisticMessage, ex, cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Исполнение метода в очереди с сохранением статистики
    /// </summary>
    /// <param name="method"></param>
    /// <param name="statisticMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Message> ExecuteEnqueueSafety(Task<Message> method, StatisticMessage statisticMessage, CancellationToken cancellationToken)
    {
        try
        {
            var result = await Enqueue(() => method, cancellationToken);
            await telegramStatisticContext.HandleStatisticAsync(statisticMessage, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            await telegramStatisticContext.HandleErrorStatisticAsync(statisticMessage, ex, cancellationToken);
            throw;
        }
    }
}