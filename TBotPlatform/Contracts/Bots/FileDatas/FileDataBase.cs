namespace TBotPlatform.Contracts.Bots.FileDatas;

public class FileDataBase
{
    /// <summary>
    /// File name
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// The file content
    /// </summary>
    public byte[] Bytes { get; set; } = null!;
}