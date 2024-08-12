namespace TBotPlatform.Contracts.Bots.Chats;

public class TelegramChatPermissions(
    bool? canSendMessages,
    bool? canSendAudios,
    bool? canSendDocuments,
    bool? canSendPhotos,
    bool? canSendVideos,
    bool? canSendVideoNotes,
    bool? canSendVoiceNotes,
    bool? canSendPolls,
    bool? canSendOtherMessages,
    bool? canAddWebPagePreviews,
    bool? canChangeInfo,
    bool? canInviteUsers,
    bool? canPinMessages,
    bool? canManageTopics
    )
{
    /// <summary>
    /// Optional. <see langword="true"/>, if the user is allowed to send text messages, contacts, locations and venues
    /// </summary>
    public bool? CanSendMessages { get; } = canSendMessages;

    /// <summary>
    /// Optional. <see langword="true" />, if the user is allowed to send audios
    /// </summary>
    public bool? CanSendAudios { get; } = canSendAudios;

    /// <summary>
    /// Optional. <see langword="true" />, if the user is allowed to send documents
    /// </summary>
    public bool? CanSendDocuments { get; } = canSendDocuments;

    /// <summary>
    /// Optional. <see langword="true" />, if the user is allowed to send photos
    /// </summary>
    public bool? CanSendPhotos { get; } = canSendPhotos;

    /// <summary>
    /// Optional. <see langword="true" />, if the user is allowed to send videos
    /// </summary>
    public bool? CanSendVideos { get; } = canSendVideos;

    /// <summary>
    /// Optional. <see langword="true" />, if the user is allowed to send video notes
    /// </summary>
    public bool? CanSendVideoNotes { get; } = canSendVideoNotes;

    /// <summary>
    /// Optional. <see langword="true" />, if the user is allowed to send voice notes
    /// </summary>
    public bool? CanSendVoiceNotes { get; } = canSendVoiceNotes;

    /// <summary>
    /// Optional. <see langword="true"/>, if the user is allowed to send polls, implies <see cref="CanSendMessages"/>
    /// </summary>
    public bool? CanSendPolls { get; } = canSendPolls;

    /// <summary>
    /// Optional. <see langword="true"/>, if the user is allowed to send animations, games, stickers and use inline
    /// bots
    /// </summary>
    public bool? CanSendOtherMessages { get; } = canSendOtherMessages;

    /// <summary>
    /// Optional. <see langword="true"/>, if the user is allowed to add web page previews to their messages
    /// </summary>
    public bool? CanAddWebPagePreviews { get; } = canAddWebPagePreviews;

    /// <summary>
    /// Optional. <see langword="true"/>, if the user is allowed to change the chat title, photo and other settings.
    /// Ignored in public supergroups
    /// </summary>
    public bool? CanChangeInfo { get; } = canChangeInfo;

    /// <summary>
    /// Optional. <see langword="true"/>, if the user is allowed to invite new users to the chat
    /// </summary>
    public bool? CanInviteUsers { get; } = canInviteUsers;

    /// <summary>
    /// Optional. <see langword="true"/>, if the user is allowed to pin messages. Ignored in public supergroups
    /// </summary>
    public bool? CanPinMessages { get; } = canPinMessages;

    /// <summary>
    /// Optional. <see langword="true"/>, if the user is allowed to create forum topics.
    /// If omitted defaults to the value of <see cref="CanPinMessages"/>
    /// supergroups only
    /// </summary>
    public bool? CanManageTopics { get; } = canManageTopics;
}