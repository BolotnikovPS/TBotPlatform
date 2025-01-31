namespace TBotPlatform.Contracts.Bots.FileDatas;

public class FileData : FileDataBase
{
    /// <summary>
    /// Размер файла
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public string FileId { get; set; }
}