namespace TBotPlatform.Contracts.Bots.FileDatas;

public class FileDataBase
{
    /// <summary>
    /// Название файла
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Сам файл
    /// </summary>
    public byte[] Bytes { get; set; } = null!;
}