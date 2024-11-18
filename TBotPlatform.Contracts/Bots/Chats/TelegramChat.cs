#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Bots.Chats;

/// <summary>
/// Информация о чате <see cref="Chat"/>
/// </summary>
public class TelegramChat(
    long id,
    EChatType type,
    string? title,
    string? username,
    string? firstName,
    string? lastName,
    bool? isForum
    )
{
    /// <summary>
    /// Unique identifier for this chat. This number may have more
    /// than 32 significant bits and some programming languages may have
    /// difficulty/silent defects in interpreting it. But it has
    /// at most 52 significant bits, so a signed 64-bit integer
    /// or double-precision float type are safe for storing this identifier.
    /// </summary>
    public long Id { get; } = id;

    /// <summary>
    /// Type of chat, can be either “private”, “group”, “supergroup” or “channel”
    /// </summary>
    public EChatType Type { get; } = type;

    /// <summary>
    /// Optional. Title, for supergroups, channels and group chats
    /// </summary>
    public string? Title { get; } = title;

    /// <summary>
    /// Optional. Username, for private chats, supergroups and channels if available
    /// </summary>
    public string? Username { get; } = username;

    /// <summary>
    /// Optional. First name of the other party in a private chat
    /// </summary>
    public string? FirstName { get; } = firstName;

    /// <summary>
    /// Optional. Last name of the other party in a private chat
    /// </summary>
    public string? LastName { get; } = lastName;

    /// <summary>
    /// Optional. <see langword="true"/>, if the supergroup chat is a forum (has topics enabled)
    /// </summary>
    public bool? IsForum { get; } = isForum;
}