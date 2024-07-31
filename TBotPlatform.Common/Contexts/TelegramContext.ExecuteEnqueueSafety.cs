using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    /// <summary>
    /// Исполнение метода в очереди с сохранением статистики
    /// </summary>
    /// <param name="method"></param>
    /// <param name="logMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ExecuteEnqueueSafety(Task method, TelegramContextLogMessage logMessage, CancellationToken cancellationToken)
    {
        try
        {
            await Enqueue(() => method, cancellationToken);
            await telegramContextLog.HandleLogAsync(logMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            await telegramContextLog.HandleErrorLogAsync(logMessage, ex, cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Исполнение метода в очереди с сохранением статистики
    /// </summary>
    /// <param name="method"></param>
    /// <param name="logMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<T> ExecuteEnqueueSafety<T>(Task<T> method, TelegramContextLogMessage logMessage, CancellationToken cancellationToken)
    {
        try
        {
            var result = await Enqueue(() => method, cancellationToken);

            if (result.IsNotNull())
            {
                logMessage.Result = result.ToJson();
            }

            await telegramContextLog.HandleLogAsync(logMessage, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            await telegramContextLog.HandleErrorLogAsync(logMessage, ex, cancellationToken);
            throw;
        }
    }
}