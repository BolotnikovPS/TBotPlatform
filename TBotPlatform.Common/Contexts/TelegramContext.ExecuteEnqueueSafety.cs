using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Extension;

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
    private async Task ExecuteEnqueueSafety(Task method, TelegramContextLogMessage statisticMessage, CancellationToken cancellationToken)
    {
        try
        {
            await Enqueue(() => method, cancellationToken);
            await telegramContextLog.HandleLogAsync(statisticMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            await telegramContextLog.HandleErrorLogAsync(statisticMessage, ex, cancellationToken);
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
    private async Task<T> ExecuteEnqueueSafety<T>(Task<T> method, TelegramContextLogMessage statisticMessage, CancellationToken cancellationToken)
    {
        try
        {
            var result = await Enqueue(() => method, cancellationToken);

            statisticMessage.Result = result.ToJson();

            await telegramContextLog.HandleLogAsync(statisticMessage, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            await telegramContextLog.HandleErrorLogAsync(statisticMessage, ex, cancellationToken);
            throw;
        }
    }
}