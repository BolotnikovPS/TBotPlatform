#nullable enable
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using TBotPlatform.Contracts.Bots.Markups;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;

public class ChatResult(
    TelegramChat chat,
    int messageId,
    string? text,
    InlineMarkupList? inlineMarkupListOrNull,
    string? messageLinkOrNull,
    ChatEntity[]? messageEntities = null
    )
{
    /// <summary>
    /// Conversation the message belongs to
    /// </summary>
    public TelegramChat Chat { get; } = chat;

    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public int MessageId { get; } = messageId;

    /// <summary>
    /// Сообщение от пользователя
    /// </summary>
    public string Text { get; } = text ?? "";

    /// <summary>
    /// Данные о inline кнопках
    /// </summary>
    public InlineMarkupList? InlineMarkupListOrNull { get; } = inlineMarkupListOrNull;

    /// <summary>
    /// Данные об объектах сообщения
    /// </summary>
    public ChatEntity[]? MessageEntities { get; } = messageEntities;

    /// <summary>
    /// Ссылка на сообщения если сообщение не с <see cref="EChatType.Supergroup"/> или <see cref="EChatType.Channel"/>, иначе null
    /// </summary>
    public string? MessageLinkOrNull { get; } = messageLinkOrNull;
}