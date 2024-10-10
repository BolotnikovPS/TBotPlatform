namespace TBotPlatform.Contracts.Bots.FileDatas;

public class FileDataBase
{
    /// <summary>
    /// Название файла
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Сам файл
    /// </summary>
    public byte[] Bytes { get; set; }
}