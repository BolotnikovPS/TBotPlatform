#nullable enable
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatUpdateForwardFromUser(TelegramUser? fromUserOrNull, bool isForwardFromBot, TelegramChat chat, int? forwardFromMessageIdOrNull, string? forwardFromSignatureOrNull, string? forwardFromSenderNameOrNull, DateTime? forwardFromDateOrNull)
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public TelegramUser? FromUserOrNull { get; } = fromUserOrNull;

    public bool IsForwardFromBot { get; } = isForwardFromBot;

    public TelegramChat Chat { get; } = chat;

    /// <summary>
    /// Id пересылаемого сообщения
    /// </summary>
    public int? FromMessageIdOrNull { get; } = forwardFromMessageIdOrNull;

    /// <summary>
    /// Сигнатура пересылаемого сообщения
    /// </summary>
    public string? SignatureOrNull { get; } = forwardFromSignatureOrNull;

    /// <summary>
    /// Имя владельца сообщения
    /// </summary>
    public string? SenderNameOrNull { get; } = forwardFromSenderNameOrNull;

    /// <summary>
    /// Время создания сообщения
    /// </summary>
    public DateTime? DateOrNull { get; } = forwardFromDateOrNull;
}