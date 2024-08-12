#nullable enable
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class TelegramMessageUserData(TelegramUser? userOrNull, TelegramChat? chatOrNull)
{
    /// <summary>
    /// Информация о пользователе telegram
    /// Не возвращает информацию если UpdateType = Unknown, Poll, PollAnswer
    /// </summary>
    public TelegramUser? UserOrNull { get; } = userOrNull;

    /// <summary>
    /// Информация о чате telegram
    /// Не возвращает информацию если UpdateType = Unknown, Poll, PollAnswer, InlineQuery, ChosenInlineResult, ShippingQuery, PreCheckoutQuery
    /// </summary>
    public TelegramChat? ChatOrNull { get; } = chatOrNull;
}