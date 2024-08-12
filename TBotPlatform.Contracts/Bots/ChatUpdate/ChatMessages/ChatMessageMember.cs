using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

public class ChatMessageMember(ChatMemberStatus status)
{
    public ChatMemberStatus Status { get; } = status;
}