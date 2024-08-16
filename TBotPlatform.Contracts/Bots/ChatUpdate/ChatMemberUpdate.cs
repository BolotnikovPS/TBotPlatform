#nullable enable
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatMemberUpdate(TelegramChat? chatOrNull, TelegramUser fromUser, DateTime date, ChatMemberData oldChatMember, ChatMemberData newChatMember)
{
    public TelegramChat? ChatOrNull { get; } = chatOrNull;

    /// <summary>
    /// Данные пользователя
    /// </summary>
    public TelegramUser FromUser { get; } = fromUser;

    /// <summary>
    /// Дата запроса
    /// </summary>
    public DateTime Date { get; } = date;

    /// <summary>
    /// Предыдущее состояние действия
    /// </summary>
    public ChatMemberData OldChatMember { get; } = oldChatMember;

    /// <summary>
    /// Новое состояние действия
    /// </summary>
    public ChatMemberData NewChatMember { get; } = newChatMember;
}