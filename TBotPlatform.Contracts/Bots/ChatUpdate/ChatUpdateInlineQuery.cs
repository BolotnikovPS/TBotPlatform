#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatUpdateInlineQuery(string id, TelegramUser fromUser, string query, string offset, EChatType chatType)
{
    /// <summary>
    /// Unique identifier for this query
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// Данные пользователя
    /// </summary>
    public TelegramUser FromUser { get; } = fromUser;

    /// <summary>
    /// Text of the query (up to 256 characters)
    /// </summary>
    public string Query { get; } = query;

    /// <summary>
    /// Offset of the results to be returned, can be controlled by the bot
    /// </summary>
    public string Offset { get; } = offset;

    /// <summary>
    /// Optional. Type of the chat, from which the inline query was sent.
    /// </summary>
    public EChatType ChatType { get; } = chatType;
}