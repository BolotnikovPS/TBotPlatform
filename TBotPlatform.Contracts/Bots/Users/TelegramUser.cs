#nullable enable
using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots.Users;

public class TelegramUser(
    long tgUserId,
    string userName,
    string firstName,
    string lastName,
    bool isBot,
    string? languageCode,
    bool? isPremium,
    bool? addedToAttachmentMenu,
    bool? canJoinGroups,
    bool? canReadAllGroupMessages,
    bool? supportsInlineQueries
    )
{
    public long TgUserId { get; } = tgUserId;

    public string UserName { get; } = userName;

    public string FirstName { get; } = firstName;

    public string LastName { get; } = lastName;

    /// <summary>
    /// <see langword="true"/>, if this user is a bot
    /// </summary>
    public bool IsBot { get; } = isBot;

    /// <summary>
    /// Optional. <a href="https://en.wikipedia.org/wiki/IETF_language_tag">IETF language tag</a> of the
    /// user's language
    /// </summary>
    public string? LanguageCode { get; } = languageCode;

    /// <summary>
    /// Optional. <see langword="true"/>, if this user is a Telegram Premium user
    /// </summary>
    public bool? IsPremium { get; } = isPremium;

    /// <summary>
    /// Optional. <see langword="true"/>, if this user added the bot to the attachment menu
    /// </summary>
    public bool? AddedToAttachmentMenu { get; } = addedToAttachmentMenu;

    /// <summary>
    /// Optional. <see langword="true"/>, if the bot can be invited to groups. Returned only in <see cref="Requests.GetMeRequest"/>
    /// </summary>
    public bool? CanJoinGroups { get; } = canJoinGroups;

    /// <summary>
    /// Optional. <see langword="true"/>, if privacy mode is disabled for the bot. Returned only in <see cref="Requests.GetMeRequest"/>
    /// </summary>
    public bool? CanReadAllGroupMessages { get; } = canReadAllGroupMessages;

    /// <summary>
    /// Optional. <see langword="true"/>, if the bot supports inline queries. Returned only in <see cref="Requests.GetMeRequest"/>
    /// </summary>
    public bool? SupportsInlineQueries { get; } = supportsInlineQueries;

    /// <inheritdoc/>
    public override string ToString()
        => $"{(UserName.IsNull() ? $"{FirstName}{LastName.Insert(0, " ")}" : $"@{UserName}")} ({TgUserId})";
}