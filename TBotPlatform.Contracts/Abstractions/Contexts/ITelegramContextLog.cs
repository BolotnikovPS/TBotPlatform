using TBotPlatform.Contracts.Statistics;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface ITelegramContextLog
{
    /// <summary>
    /// Сохраняет информацию о взаимодействии с telegram
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleLog(TelegramContextFullLogMessage message, CancellationToken cancellationToken);

    /// <summary>
    /// Сохраняет информацию о взаимодействии с telegram в случае вызова исключения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="exception">Исключение</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleErrorLog(TelegramContextFullLogMessage message, Exception exception, CancellationToken cancellationToken);

    /// <summary>
    /// Логирование данных по запросам к telegram
    /// </summary>
    /// <param name="requestCount">Число запросов</param>
    /// <param name="elapsedMilliseconds">Общее время на все запросы</param>
    /// <param name="operationGuid">Id текущих операций</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleEnqueueLog(int requestCount, int elapsedMilliseconds, Guid operationGuid, CancellationToken cancellationToken);
}