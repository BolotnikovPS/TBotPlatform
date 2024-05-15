namespace TBotPlatform.Contracts.Bots;

public class FileData
{
    /// <summary>
    /// Название файла
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Сам файл
    /// </summary>
    public byte[] Byte { get; set; }

    /// <summary>
    /// Размер файла
    /// </summary>
    public long Size { get; set; }
}