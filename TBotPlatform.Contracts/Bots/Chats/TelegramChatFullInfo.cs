#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Bots.Chats;

/// <summary>
/// Полная информация о чате <see cref="ChatFullInfo"/>
/// </summary>
public class TelegramChatFullInfo(
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
    TelegramChatPinMessage? pinnedMessageOrNull,
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
    ) : TelegramChat(
    id,
    type,
    title,
    username,
    firstName,
    lastName,
    isForum
    )
{
    public TelegramChatPhoto? PhotoOrNull { get; } = photoOrNull;

    public string[]? ActiveUsernames { get; } = activeUsernames;

    public string? EmojiStatusCustomEmojiId { get; } = emojiStatusCustomEmojiId;

    public string? Bio { get; } = bio;

    public bool? HasPrivateForwards { get; } = hasPrivateForwards;

    public bool? HasRestrictedVoiceAndVideoMessages { get; } = hasRestrictedVoiceAndVideoMessages;

    public bool? JoinToSendMessages { get; } = joinToSendMessages;

    public bool? JoinByRequest { get; } = joinByRequest;

    public string? Description { get; } = description;

    public string? InviteLink { get; } = inviteLink;

    public TelegramChatPinMessage? PinnedMessageOrNull { get; } = pinnedMessageOrNull;

    public TelegramChatPermissions? PermissionsOrNull { get; } = permissionsOrNull;

    public int? SlowModeDelay { get; } = slowModeDelay;

    public int? MessageAutoDeleteTime { get; } = messageAutoDeleteTime;

    public bool? HasAggressiveAntiSpamEnabled { get; } = hasAggressiveAntiSpamEnabled;

    public bool? HasHiddenMembers { get; } = hasHiddenMembers;

    public bool? HasProtectedContent { get; } = hasProtectedContent;

    public string? StickerSetName { get; } = stickerSetName;

    public bool? CanSetStickerSet { get; } = canSetStickerSet;

    public long? LinkedChatId { get; } = linkedChatId;

    public TelegramChatLocation? LocationOrNull { get; } = locationOrNull;
}