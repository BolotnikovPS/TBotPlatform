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
    Task HandleLogAsync(TelegramContextLogMessage message, CancellationToken cancellationToken);

    /// <summary>
    /// Сохраняет информацию о взаимодействии с telegram в случае вызова исключения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="exception">Исключение</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleErrorLogAsync(TelegramContextLogMessage message, Exception exception, CancellationToken cancellationToken);
}