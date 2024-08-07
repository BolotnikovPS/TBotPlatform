#nullable enable
namespace TBotPlatform.Contracts.Bots.ChatMessages;

public class ChatUpdateCallbackQuery(
    string callbackQueryTextOrNull,
    bool callbackQueryMessageWithImage,
    int callbackQueryMessageIdOrNull,
    DateTime callbackQueryDateIdOrNull
    )
{
    /// <summary>
    /// Сообщение от пользователя c inline кнопки
    /// </summary>
    public string CallbackQueryText { get; } = callbackQueryTextOrNull;

    /// <summary>
    /// Наличие изображения в сообщение с inline кнопками
    /// </summary>
    public bool CallbackQueryMessageWithImage { get; } = callbackQueryMessageWithImage;

    /// <summary>
    /// Id сообщения от пользователя c inline кнопки
    /// </summary>
    public int CallbackQueryMessageId { get; } = callbackQueryMessageIdOrNull;

    /// <summary>
    /// Дата сообщения от пользователя c inline кнопки
    /// </summary>
    public DateTime CallbackQueryDateId { get; } = callbackQueryDateIdOrNull;
}