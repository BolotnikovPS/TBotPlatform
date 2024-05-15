using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions;

public interface IStartReceivingHandler
{
    /// <summary>
    /// Обрабатывает данные при поступлении сообщения
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}