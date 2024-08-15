#nullable enable
namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatUpdateCallbackQuery(string text, bool messageWithImage, int messageIdOrNull, DateTime dateOrNull)
{
    /// <summary>
    /// Сообщение от пользователя c inline кнопки
    /// </summary>
    public string Text { get; } = text;

    /// <summary>
    /// Наличие изображения в сообщение с inline кнопками
    /// </summary>
    public bool MessageWithImage { get; } = messageWithImage;

    /// <summary>
    /// Id сообщения от пользователя c inline кнопки
    /// </summary>
    public int MessageId { get; } = messageIdOrNull;

    /// <summary>
    /// Дата сообщения от пользователя c inline кнопки
    /// </summary>
    public DateTime Date { get; } = dateOrNull;
}