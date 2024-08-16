#nullable enable
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;

public class ChatResult(
    TelegramChat chat,
    int messageId,
    string? text,
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

    public ChatEntity[]? MessageEntities { get; } = messageEntities;
}