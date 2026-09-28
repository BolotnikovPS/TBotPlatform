namespace TBotPlatform.Contracts.Bots.FileDatas;

/// <summary>
/// Тип файлов, отправляемых одним альбомом через <c>SendMediaGroup</c>
/// </summary>
public enum MediaGroupType
{
    /// <summary>
    /// Изображение. Соответствует <c>InputMediaPhoto</c>
    /// </summary>
    Photo = 0,

    /// <summary>
    /// Видео. Соответствует <c>InputMediaVideo</c>
    /// </summary>
    Video = 1,

    /// <summary>
    /// Документ. Соответствует <c>InputMediaDocument</c>
    /// </summary>
    Document = 2,
}
