using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

public class ChatMemberData(EChatMemberStatus status)
{
    public EChatMemberStatus Status { get; } = status;
}