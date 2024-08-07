#nullable enable
namespace TBotPlatform.Contracts.Bots.ChatMessages;

public class ChatUpdateMessage(
    int? messageId,
    string? text,
    string? replyToMessageOrNull,
    FileData? photoData,
    FileData? documentData
    )
{
    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public int? MessageId { get; } = messageId;

    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public string? Text { get; } = text;

    /// <summary>
    /// Сообщение от пользователя со ссылкой на другое сообщение
    /// </summary>
    public string? ReplyToMessageOrNull { get; } = replyToMessageOrNull;

    /// <summary>
    /// Фото присланные от пользователя в ответ на сообщение
    /// </summary>
    public FileData? PhotoDataOrNull { get; } = photoData;

    /// <summary>
    /// Документы присланные от пользователя в ответ на сообщение
    /// </summary>
    public FileData? DocumentDataOrNull { get; } = documentData;
}