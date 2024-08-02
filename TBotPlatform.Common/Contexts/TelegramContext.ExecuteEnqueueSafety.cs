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
        var fullLogMessage = new TelegramContextFullLogMessage { Request = logMessage, };

        try
        {
            await Enqueue(() => method, cancellationToken);
            await telegramContextLog.HandleLogAsync(fullLogMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            await telegramContextLog.HandleErrorLogAsync(fullLogMessage, ex, cancellationToken);
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
        var fullLogMessage = new TelegramContextFullLogMessage { Request = logMessage, };

        try
        {
            var result = await Enqueue(() => method, cancellationToken);

            if (result.IsNotNull())
            {
                fullLogMessage.Result = result.ToJson();
            }

            await telegramContextLog.HandleLogAsync(fullLogMessage, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            await telegramContextLog.HandleErrorLogAsync(fullLogMessage, ex, cancellationToken);
            throw;
        }
    }
}