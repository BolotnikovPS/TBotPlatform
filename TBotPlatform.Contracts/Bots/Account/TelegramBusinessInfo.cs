using TBotPlatform.Contracts.Bots.Users;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Bots.Account;

/// <summary>
/// Информация о бизнес аккаунте <see cref="BusinessConnection"/>
/// </summary>
public class TelegramBusinessInfo(
    string id,
    TelegramUser user,
    long userChatId,
    DateTime dateConnection,
    bool canReply,
    bool isEnabled
    )
{
    public string Id { get; } = id;

    public TelegramUser User { get; } = user;

    public long UserChatId { get; } = userChatId;

    public DateTime DateConnection { get; } = dateConnection;

    public bool CanReply { get; } = canReply;

    public bool IsEnabled { get; } = isEnabled;
}