#nullable enable
using TBotPlatform.Contracts.Bots.UserBases;

namespace TBotPlatform.Contracts.Bots.ChatMessages;

public class ChatMessageForwardFromUser(UserMinimal? user, UserMinimal? bot, int? forwardFromMessageId, string? forwardFromSignature, string? forwardFromSenderName, DateTime? forwardFromDate)
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public UserMinimal? User { get; } = user;

    /// <summary>
    /// Данные пользователя
    /// </summary>
    public UserMinimal? Bot { get; } = user;

    /// <summary>
    /// Id пересылаемого сообщения
    /// </summary>
    public int? MessageId { get; } = forwardFromMessageId;

    /// <summary>
    /// Сигнатура пересылаемого сообщения
    /// </summary>
    public string? Signature { get; } = forwardFromSignature;

    /// <summary>
    /// Имя владельца сообщения
    /// </summary>
    public string? SenderName { get; } = forwardFromSenderName;

    /// <summary>
    /// Время создания сообщения
    /// </summary>
    public DateTime? Date { get; } = forwardFromDate;
}