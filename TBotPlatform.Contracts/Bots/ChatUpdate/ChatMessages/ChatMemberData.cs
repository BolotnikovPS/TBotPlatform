#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

public class ChatMemberData(EChatMemberStatus status, TelegramUser? fromUserOrNull = null)
{
    public EChatMemberStatus Status { get; } = status;

    public TelegramUser? FromUserOrNull { get; } = fromUserOrNull;
}