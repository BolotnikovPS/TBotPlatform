#nullable enable
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatMessages;

namespace TBotPlatform.Contracts.Abstractions;

public interface IStartReceivingHandler
{
    /// <summary>
    /// Обрабатывает данные при поступлении сообщения
    /// </summary>
    /// <param name="chatMessage">Сообщение с telegram</param>
    /// <param name="markupNextState"></param>
    /// <param name="telegramUser"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleUpdateAsync(ChatMessage chatMessage, MarkupNextState? markupNextState, TelegramUser telegramUser, CancellationToken cancellationToken);
}