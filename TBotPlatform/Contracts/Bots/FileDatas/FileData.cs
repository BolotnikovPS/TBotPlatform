namespace TBotPlatform.Contracts.Bots.FileDatas;

public class FileData : FileDataBase
{
    /// <summary>
    /// File size
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// File identifier
    /// </summary>
    public string FileId { get; set; } = null!;
}