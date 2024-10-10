#nullable enable
namespace TBotPlatform.Contracts.Bots.Chats;

public class TelegramChatPinMessage(int messageId, DateTime date, string? text)
{
    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public int MessageId { get; } = messageId;


    /// <summary>
    /// Дата, когда сообщение было отправлено
    /// </summary>
    public DateTime Date { get; } = date;

    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public string Text { get; } = text ?? "";
}