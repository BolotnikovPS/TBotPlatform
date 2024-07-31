#nullable enable
using TBotPlatform.Contracts.Bots;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions;

public interface IStartReceivingHandler
{
    /// <summary>
    /// Обрабатывает данные при поступлении сообщения
    /// </summary>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="markupNextState"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleUpdateAsync(Update update, MarkupNextState? markupNextState, CancellationToken cancellationToken);
}