using TBotPlatform.Contracts.Statistics;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface ITelegramContextLog
{
    /// <summary>
    /// Saves the information about interaction with Telegram
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="cancellationToken"></param>
    Task HandleLog(TelegramContextFullLogMessage message, CancellationToken cancellationToken);

    /// <summary>
    /// Saves the information about interaction with Telegram when an exception is raised
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="exception">The exception</param>
    /// <param name="cancellationToken"></param>
    Task HandleErrorLog(TelegramContextFullLogMessage message, Exception exception, CancellationToken cancellationToken);

    /// <summary>
    /// Logs data about requests to Telegram
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="requestCount">Number of requests</param>
    /// <param name="elapsedMilliseconds">Total time spent on all requests</param>
    /// <param name="operationGuid">Id of the current operations</param>
    Task HandleEnqueueLog(int requestCount, int elapsedMilliseconds, Guid operationGuid, CancellationToken cancellationToken);
}