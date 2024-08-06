using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts.Bots.ChatMessages;

public class ChatMessageMember(ChatMemberStatus status)
{
    public ChatMemberStatus Status { get; } = status;
}