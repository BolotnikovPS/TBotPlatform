#nullable enable
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

public class ChatForward(
    TelegramUser fromUser,
    bool isForwardFromBot,
    TelegramChat chat,
    int? forwardFromMessageId,
    string? forwardFromSignature,
    string? forwardFromSenderName,
    DateTime? forwardFromDate
    )
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public TelegramUser FromUser { get; } = fromUser;

    public bool IsForwardFromBot { get; } = isForwardFromBot;

    public TelegramChat Chat { get; } = chat;

    /// <summary>
    /// Id пересылаемого сообщения
    /// </summary>
    public int? FromMessageId { get; } = forwardFromMessageId;

    /// <summary>
    /// Сигнатура пересылаемого сообщения
    /// </summary>
    public string Signature { get; } = forwardFromSignature ?? "";

    /// <summary>
    /// Имя владельца сообщения
    /// </summary>
    public string SenderName { get; } = forwardFromSenderName ?? "";

    /// <summary>
    /// Время создания сообщения
    /// </summary>
    public DateTime Date { get; } = forwardFromDate ?? DateTime.UtcNow;
}