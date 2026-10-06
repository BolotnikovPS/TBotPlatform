namespace TBotPlatform.Contracts.Bots.FileDatas;

/// <summary>
/// Type of files sent as a single album via <c>SendMediaGroup</c>
/// </summary>
public enum MediaGroupType
{
    /// <summary>
    /// Image. Corresponds to <c>InputMediaPhoto</c>
    /// </summary>
    Photo = 0,

    /// <summary>
    /// Video. Corresponds to <c>InputMediaVideo</c>
    /// </summary>
    Video = 1,

    /// <summary>
    /// Document. Corresponds to <c>InputMediaDocument</c>
    /// </summary>
    Document = 2,
}
