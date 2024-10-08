﻿namespace TBotPlatform.Contracts.Bots.Chats;

public class TelegramChatPhoto(string smallFileId, string smallFileUniqueId, string bigFileId, string bigFileUniqueId)
{
    /// <summary>
    /// File identifier of small (160x160) chat photo. This FileId can be used only for photo download and only
    /// for as long as the photo is not changed.
    /// </summary>
    public string SmallFileId { get; } = smallFileId;

    /// <summary>
    /// Unique file identifier of small (160x160) chat photo, which is supposed to be the same over time and for
    /// different bots. Can't be used to download or reuse the file.
    /// </summary>
    public string SmallFileUniqueId { get; } = smallFileUniqueId;

    /// <summary>
    /// File identifier of big (640x640) chat photo. This FileId can be used only for photo download and only for
    /// as long as the photo is not changed.
    /// </summary>
    public string BigFileId { get; } = bigFileId;

    /// <summary>
    /// Unique file identifier of big (640x640) chat photo, which is supposed to be the same over time and for
    /// different bots. Can't be used to download or reuse the file.
    /// </summary>
    public string BigFileUniqueId { get; } = bigFileUniqueId;
}