#nullable enable
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Bots.Locations;
using TBotPlatform.Contracts.Bots.Markups;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatMessage(
    EChatMessageType type,
    TelegramChat chat,
    int messageId,
    DateTime date,
    string? text,
    ChatReplyToMessage? replyToMessageOrNull,
    FileData? photoData,
    FileData? documentData,
    ChatForward? forwardOrNull = null,
    TelegramChat? senderChatOrNull = null,
    TelegramLocation? locationOrNull = null,
    InlineMarkupList? inlineMarkupListOrNull = null,
    ChatWebAppData? webAppDataOrNull = null,
    ChatEntity[]? entities = null
    )
{
    public EChatMessageType Type { get; } = type;

    /// <summary>
    /// Conversation the message belongs to
    /// </summary>
    public TelegramChat Chat { get; } = chat;

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

    /// <summary>
    /// Фото присланные от пользователя в ответ на сообщение
    /// </summary>
    public FileData? PhotoDataOrNull { get; } = photoData;

    /// <summary>
    /// Документы присланные от пользователя в ответ на сообщение
    /// </summary>
    public FileData? DocumentDataOrNull { get; } = documentData;

    /// <summary>
    /// Сообщение от пользователя со ссылкой на другое сообщение
    /// </summary>
    public ChatReplyToMessage? ReplyToMessageOrNull { get; } = replyToMessageOrNull;

    /// <summary>
    /// Сообщение пересланное от пользователя
    /// </summary>
    public ChatForward? ForwardOrNull { get; } = forwardOrNull;

    /// <summary>
    /// Optional. Sender of the message, sent on behalf of a chat. The channel itself for channel messages.
    /// The supergroup itself for messages from anonymous group administrators. The linked channel for messages
    /// automatically forwarded to the discussion group
    /// </summary>
    public TelegramChat? SenderChatOrNull { get; } = senderChatOrNull;

    /// <summary>
    /// Данные о локации
    /// </summary>
    public TelegramLocation? LocationOrNull { get; } = locationOrNull;

    /// <summary>
    /// Данные о inline кнопках
    /// </summary>
    public InlineMarkupList? InlineMarkupListOrNull { get; } = inlineMarkupListOrNull;

    /// <summary>
    /// Данные с кнопки WebApp
    /// </summary>
    public ChatWebAppData? WebAppDataOrNull { get; } = webAppDataOrNull;

    /// <summary>
    /// Данные об объектах сообщения
    /// </summary>
    public ChatEntity[]? Entities { get; } = entities;
}