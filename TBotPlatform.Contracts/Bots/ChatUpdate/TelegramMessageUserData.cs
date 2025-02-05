#nullable enable
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class TelegramMessageUserData(User? userOrNull, Chat? chatOrNull)
{
    /// <summary>
    /// Информация о пользователе telegram
    /// Не возвращает информацию если UpdateType = Unknown, Poll, PollAnswer
    /// </summary>
    public User? UserOrNull { get; } = userOrNull;

    /// <summary>
    /// Информация о чате telegram
    /// Не возвращает информацию если UpdateType = Unknown, Poll, PollAnswer, InlineQuery, ChosenInlineResult, ShippingQuery, PreCheckoutQuery
    /// </summary>
    public Chat? ChatOrNull { get; } = chatOrNull;
}