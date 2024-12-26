#nullable enable
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatChosenInlineResult(string resultId, TelegramUser fromUser, string? inlineMessageId, string query)
{
    /// <summary>
    /// The unique identifier for the result that was chosen.
    /// </summary>
    public string ResultId { get; } = resultId;

    /// <summary>
    /// Данные пользователя
    /// </summary>
    public TelegramUser FromUser { get; } = fromUser;

    /// <summary>
    /// Optional. Identifier of the send inline message. Available only if there is an inline keyboard attached
    /// to the message. Will be also received in callback queries and can be used to edit the message.
    /// </summary>
    public string? InlineMessageId { get; } = inlineMessageId;

    /// <summary>
    /// The query that was used to obtain the result.
    /// </summary>
    public string Query { get; } = query;
}