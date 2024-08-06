using TBotPlatform.Contracts.Bots.UserBases;

namespace TBotPlatform.Contracts.Bots.ChatMessages;

public class ChatMessageMemberUpdate(long? chatId, UserMinimal user, DateTime date, ChatMessageMember oldChatMember, ChatMessageMember newChatMember)
{
    public long? ChatId { get; } = chatId;

    /// <summary>
    /// Данные пользователя
    /// </summary>
    public UserMinimal User { get; } = user;

    public DateTime Date { get; } = date;

    public ChatMessageMember OldChatMember { get; } = oldChatMember;

    public ChatMessageMember NewChatMember { get; } = newChatMember;
}