#nullable enable
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts.Bots.ChatMessages;

public class ChatMessage(
    UpdateType updateType,
    int? messageId,
    string? message,
    string? replyToMessageOrNull,
    ChatMessageForwardFromUser? forwardFromUserOrNull,
    ChatMessageCallbackQuery? callbackQueryOrNull,
    ChatMessageMemberUpdate? myChatMessageMemberUpdate,
    FileData? photoData,
    FileData? documentData
    )
{
    public UpdateType UpdateType { get; } = updateType;

    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public int? MessageId { get; } = messageId;

    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public string? Message { get; } = message;

    /// <summary>
    /// Сообщение от пользователя со ссылкой на другое сообщение
    /// </summary>
    public string? ReplyToMessageOrNull { get; } = replyToMessageOrNull;

    /// <summary>
    /// Сообщение от пользователя c inline кнопки
    /// </summary>
    public ChatMessageCallbackQuery? CallbackQueryOrNull { get; } = callbackQueryOrNull;
    
    /// <summary>
    /// Сообщение пересланное от пользователя
    /// </summary>
    public ChatMessageForwardFromUser? ForwardFromUserOrNull { get; } = forwardFromUserOrNull;

    public ChatMessageMemberUpdate? MyChatMessageMemberUpdateOrNull { get; } = myChatMessageMemberUpdate;

    /// <summary>
    /// Фото присланные от пользователя в ответ на сообщение
    /// </summary>
    public FileData? PhotoDataOrNull { get; } = photoData;

    /// <summary>
    /// Документы присланные от пользователя в ответ на сообщение
    /// </summary>
    public FileData? DocumentDataOrNull { get; } = documentData;
}