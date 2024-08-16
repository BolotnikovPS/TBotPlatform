using TBotPlatform.Contracts.Bots.ChatUpdate;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Handlers;

public interface ITelegramUpdateHandler
{
    /// <summary>
    /// Получает пользователя telegram
    /// </summary>
    /// <param name="update">Сообщение с telegram</param>
    /// <returns></returns>
    TelegramMessageUserData GetTelegramMessageUserData(Update update);

    /// <summary>
    /// Получает форматированное сообщение с telegram
    /// </summary>
    /// <param name="telegramUser">Пользователь telegram</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatUpdate> GetChatUpdateAsync(TelegramMessageUserData telegramUser, Update update, CancellationToken cancellationToken);

    /// <summary>
    /// Получает форматированное сообщение с telegram
    /// </summary>
    /// <param name="chatId">Id чата пользователя</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatUpdate> GetChatMessageAsync(long chatId, Update update, CancellationToken cancellationToken);
}