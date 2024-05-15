using TBotPlatform.Extension;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Bots;

public class ChatMessage
{
    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Сообщение от пользователя с ссылкой на другое сообщение
    /// </summary>
    public string ReplyToMessageOrNull { get; }

    /// <summary>
    /// Сообщение от пользователя c inline кнопки
    /// </summary>
    public string CallbackQueryMessageOrNull { get; }

    /// <summary>
    /// Наличие изображения в сообщение с inline кнопками
    /// </summary>
    public bool CallbackQueryMessageWithCaption { get; }

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

    public ChatMessage(
        string message,
        string replyToMessageOrNull,
        CallbackQuery callbackQuery,
        FileData photoData
        )
    {
        Message = message;
        ReplyToMessageOrNull = replyToMessageOrNull;
        PhotoData = photoData;

        CallbackQueryMessageWithCaption = callbackQuery?.Message?.Caption?.CheckAny() ?? false;
        CallbackQueryMessageOrNull = CallbackQueryMessageWithCaption ? callbackQuery?.Message?.Caption : callbackQuery?.Message?.Text;
        CallbackQueryMessageIdOrNull = callbackQuery?.Message?.MessageId;
        CallbackQueryDateIdOrNull = callbackQuery?.Message?.Date;
    }
}