using TBotPlatform.Contracts.Bots.ChatMessages;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions;

public interface ITelegramUpdateHandler
{
    /// <summary>
    /// Получает пользователя telegram
    /// </summary>
    /// <param name="update">Сообщение с telegram</param>
    /// <returns></returns>
    TelegramUser GetTelegramUser(Update update);

    /// <summary>
    /// Получает форматированное сообщение с telegram
    /// </summary>
    /// <param name="telegramUser">Пользователь telegram</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatUpdate> GetChatUpdateAsync(TelegramUser telegramUser, Update update, CancellationToken cancellationToken);

    /// <summary>
    /// Получает форматированное сообщение с telegram
    /// </summary>
    /// <param name="chatId">Id чата пользователя</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatUpdate> GetChatMessageAsync(long chatId, Update update, CancellationToken cancellationToken);
}