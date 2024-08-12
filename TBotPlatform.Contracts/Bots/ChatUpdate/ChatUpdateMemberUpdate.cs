#nullable enable
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatUpdateMemberUpdate(TelegramChat? chatOrNull, TelegramUser fromUser, DateTime date, ChatMessageMember oldChatMember, ChatMessageMember newChatMember)
{
    public TelegramChat? ChatOrNull { get; } = chatOrNull;

    /// <summary>
    /// Данные пользователя
    /// </summary>
    public TelegramUser FromUser { get; } = fromUser;

    public DateTime Date { get; } = date;

    public ChatMessageMember OldChatMember { get; } = oldChatMember;

    public ChatMessageMember NewChatMember { get; } = newChatMember;
}