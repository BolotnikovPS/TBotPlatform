using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts.Bots;

public class ChatMessage
{
    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Сообщение от пользователя со ссылкой на другое сообщение
    /// </summary>
    public string ReplyToMessageOrNull { get; }

    /// <summary>
    /// Сообщение от пользователя c inline кнопки
    /// </summary>
    public string CallbackQueryMessageOrNull { get; }

    /// <summary>
    /// Наличие изображения в сообщение с inline кнопками
    /// </summary>
    public bool CallbackQueryMessageWithImage { get; }

    /// <summary>
    /// Id сообщения от пользователя c inline кнопки
    /// </summary>
    public int? CallbackQueryMessageIdOrNull { get; }

    /// <summary>
    /// Дата сообщения от пользователя c inline кнопки
    /// </summary>
    public DateTime? CallbackQueryDateIdOrNull { get; }

    /// <summary>
    /// Фото присланные от пользователя в ответ на сообщение
    /// </summary>
    public FileData PhotoData { get; }

    /// <summary>
    /// Документы присланные от пользователя в ответ на сообщение
    /// </summary>
    public FileData DocumentData { get; }

    public ChatMessage(
        string message,
        string replyToMessageOrNull,
        CallbackQuery callbackQuery,
        FileData photoData,
        FileData documentData
        )
    {
        Message = message;
        ReplyToMessageOrNull = replyToMessageOrNull;
        PhotoData = photoData;
        DocumentData = documentData;

        CallbackQueryMessageWithImage = callbackQuery?.Message?.Type == MessageType.Photo;
        CallbackQueryMessageOrNull = CallbackQueryMessageWithImage ? callbackQuery?.Message?.Caption : callbackQuery?.Message?.Text;
        CallbackQueryMessageIdOrNull = callbackQuery?.Message?.MessageId;
        CallbackQueryDateIdOrNull = callbackQuery?.Message?.Date;
    }
}