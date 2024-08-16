using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    /// <summary>
    /// Исполнение метода в очереди с сохранением статистики
    /// </summary>
    /// <param name="method">Исполняемый метод</param>
    /// <param name="logMessage">Сообщение для логирования</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private async Task ExecuteEnqueueSafety(Task method, TelegramContextLogMessage logMessage, CancellationToken cancellationToken)
    {
        if (method.IsNull())
        {
            throw new ArgumentException(nameof(method));
        }

        if (logMessage.IsNull())
        {
            throw new ArgumentException(nameof(logMessage));
        }

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
    /// <typeparam name="T"></typeparam>
    /// <param name="method">Исполняемый метод</param>
    /// <param name="logMessage">Сообщение для логирования</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private async Task<T> ExecuteEnqueueSafety<T>(Task<T> method, TelegramContextLogMessage logMessage, CancellationToken cancellationToken)
    {
        if (method.IsNull())
        {
            throw new ArgumentException(nameof(method));
        }

        if (logMessage.IsNull())
        {
            throw new ArgumentException(nameof(logMessage));
        }

        var fullLogMessage = new TelegramContextFullLogMessage { Request = logMessage, };

        try
        {
            var result = await Enqueue(() => method, cancellationToken);

            if (result.IsNotNull())
            {
                fullLogMessage.Result = result;
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