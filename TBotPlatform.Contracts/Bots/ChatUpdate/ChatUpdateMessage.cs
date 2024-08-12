#nullable enable
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.Locations;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatUpdateMessage(
    TelegramChat chat,
    int? messageIdOrNull,
    string? text,
    ChatMessageReplyToMessage? replyToMessageOrNull,
    FileData? photoData,
    FileData? documentData,
    ChatUpdateForwardFromUser? forwardFromUserOrNull = null,
    TelegramChat? senderChatOrNull = null,
    TelegramLocation? locationOrNull = null
    )
{
    /// <summary>
    /// Conversation the message belongs to
    /// </summary>
    public TelegramChat Chat { get; } = chat;

    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public int? MessageIdOrNull { get; } = messageIdOrNull;

    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public string? Text { get; } = text;

    /// <summary>
    /// Сообщение от пользователя со ссылкой на другое сообщение
    /// </summary>
    public ChatMessageReplyToMessage? ReplyToMessageOrNull { get; } = replyToMessageOrNull;

    /// <summary>
    /// Фото присланные от пользователя в ответ на сообщение
    /// </summary>
    public FileData? PhotoDataOrNull { get; } = photoData;

    /// <summary>
    /// Документы присланные от пользователя в ответ на сообщение
    /// </summary>
    public FileData? DocumentDataOrNull { get; } = documentData;

    /// <summary>
    /// Сообщение пересланное от пользователя
    /// </summary>
    public ChatUpdateForwardFromUser? ForwardFromUserOrNull { get; } = forwardFromUserOrNull;

    /// <summary>
    /// Optional. Sender of the message, sent on behalf of a chat. The channel itself for channel messages.
    /// The supergroup itself for messages from anonymous group administrators. The linked channel for messages
    /// automatically forwarded to the discussion group
    /// </summary>
    public TelegramChat? SenderChatOrNull { get; } = senderChatOrNull;

    public TelegramLocation? LocationOrNull { get; } = locationOrNull;
}