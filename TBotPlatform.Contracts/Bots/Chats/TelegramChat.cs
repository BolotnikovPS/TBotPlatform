#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using Telegram.Bot.Requests;

namespace TBotPlatform.Contracts.Bots.Chats;

public class TelegramChat(
    long id,
    EChatType type,
    string? title,
    string? username,
    string? firstName,
    string? lastName,
    bool? isForum,
    TelegramChatPhoto? photoOrNull,
    string[]? activeUsernames,
    string? emojiStatusCustomEmojiId,
    string? bio,
    bool? hasPrivateForwards,
    bool? hasRestrictedVoiceAndVideoMessages,
    bool? joinToSendMessages,
    bool? joinByRequest,
    string? description,
    string? inviteLink,
    TelegramChatPermissions? permissionsOrNull,
    int? slowModeDelay,
    int? messageAutoDeleteTime,
    bool? hasAggressiveAntiSpamEnabled,
    bool? hasHiddenMembers,
    bool? hasProtectedContent,
    string? stickerSetName,
    bool? canSetStickerSet,
    long? linkedChatId,
    TelegramChatLocation? locationOrNull
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

    /// <summary>
    /// Optional. Chat photo. Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public TelegramChatPhoto? PhotoOrNull { get; } = photoOrNull;

    /// <summary>
    /// Optional. If non-empty, the list of all active chat usernames; for private chats, supergroups and channels.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public string[]? ActiveUsernames { get; } = activeUsernames;

    /// <summary>
    /// Optional. Custom emoji identifier of emoji status of the other party in a private chat.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public string? EmojiStatusCustomEmojiId { get; } = emojiStatusCustomEmojiId;

    /// <summary>
    /// Optional. Bio of the other party in a private chat. Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public string? Bio { get; } = bio;

    /// <summary>
    /// Optional. <see langword="true"/>, if privacy settings of the other party in the private chat allows to use
    /// <c>tg://user?id=&lt;user_id&gt;</c> links only in chats with the user.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public bool? HasPrivateForwards { get; } = hasPrivateForwards;

    /// <summary>
    /// Optional. <see langword="true"/>, if the privacy settings of the other party restrict sending voice
    /// and video note messages in the private chat.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public bool? HasRestrictedVoiceAndVideoMessages { get; } = hasRestrictedVoiceAndVideoMessages;

    /// <summary>
    /// Optional. <see langword="true"/>, if users need to join the supergroup before they can send messages.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public bool? JoinToSendMessages { get; } = joinToSendMessages;

    /// <summary>
    /// Optional. <see langword="true"/>, if all users directly joining the supergroup need to be approved by supergroup administrators.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public bool? JoinByRequest { get; } = joinByRequest;

    /// <summary>
    /// Optional. Description, for groups, supergroups and channel chats.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public string? Description { get; } = description;

    /// <summary>
    /// Optional. Primary invite link, for groups, supergroups and channel chats.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public string? InviteLink { get; } = inviteLink;

    /// <summary>
    /// Optional. The most recent pinned message (by sending date).
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    /// public ChatUpdateMessage? PinnedMessage { get; } = pinnedMessage;
    /// <summary>
    /// Optional. Default chat member permissions, for groups and supergroups.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public TelegramChatPermissions? PermissionsOrNull { get; } = permissionsOrNull;

    /// <summary>
    /// Optional. For supergroups, the minimum allowed delay between consecutive messages sent by each
    /// unpriviledged user. Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public int? SlowModeDelay { get; } = slowModeDelay;

    /// <summary>
    /// Optional. The time after which all messages sent to the chat will be automatically deleted; in seconds.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public int? MessageAutoDeleteTime { get; } = messageAutoDeleteTime;

    /// <summary>
    /// Optional. <see langword="true"/>, if aggressive anti-spam checks are enabled in the supergroup. The field is
    /// only available to chat administrators. Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public bool? HasAggressiveAntiSpamEnabled { get; } = hasAggressiveAntiSpamEnabled;

    /// <summary>
    /// Optional. <see langword="true"/>, if non-administrators can only get the list of bots and administrators in
    /// the chat. Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public bool? HasHiddenMembers { get; } = hasHiddenMembers;

    /// <summary>
    /// Optional. <see langword="true"/>, if messages from the chat can't be forwarded to other chats.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public bool? HasProtectedContent { get; } = hasProtectedContent;

    /// <summary>
    /// Optional. For supergroups, name of group sticker set.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public string? StickerSetName { get; } = stickerSetName;

    /// <summary>
    /// Optional. True, if the bot can change the group sticker set.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public bool? CanSetStickerSet { get; } = canSetStickerSet;

    /// <summary>
    /// Optional. Unique identifier for the linked chat, i.e. the discussion group identifier for a channel
    /// and vice versa; for supergroups and channel chats. This identifier may be greater than 32 bits and some
    /// programming languages may have difficulty/silent defects in interpreting it. But it is smaller than
    /// 52 bits, so a signed 64 bit integer or double-precision float type are safe for storing this identifier.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public long? LinkedChatId { get; } = linkedChatId;

    /// <summary>
    /// Optional. For supergroups, the location to which the supergroup is connected.
    /// Returned only in <see cref="GetChatRequest"/>.
    /// </summary>
    public TelegramChatLocation? LocationOrNull { get; } = locationOrNull;
}