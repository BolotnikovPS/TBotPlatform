#nullable enable
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class TelegramMessageUserData(User? userOrNull, Chat? chatOrNull)
{
    /// <summary>
    /// Information about the Telegram user
    /// Does not return information if UpdateType is Unknown, Poll, PollAnswer, StoppedMessageGeneration
    /// </summary>
    public User? UserOrNull { get; } = userOrNull;

    /// <summary>
    /// Information about the Telegram chat
    /// Does not return information if UpdateType = Unknown, Poll, PollAnswer, InlineQuery, ChosenInlineResult,
    /// ShippingQuery, PreCheckoutQuery, BusinessConnection, PurchasedPaidMedia, ManagedBot, Subscription
    /// </summary>
    public Chat? ChatOrNull { get; } = chatOrNull;
}