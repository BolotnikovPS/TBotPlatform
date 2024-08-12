#nullable enable
using TBotPlatform.Contracts.Bots.Users;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

public class CharMessageEntity(MessageEntityType type, int offset, int length, string? urlOrNull, TelegramUser? fromUserOrNull)
{
    /// <summary>
    /// Type of the entity
    /// </summary>
    public MessageEntityType Type { get; } = type;

    /// <summary>
    /// Offset in UTF-16 code units to the start of the entity
    /// </summary>
    public int Offset { get; } = offset;

    /// <summary>
    /// Length of the entity in UTF-16 code units
    /// </summary>
    public int Length { get; } = length;

    /// <summary>
    /// Optional. For <see cref="MessageEntityType.TextLink"/> only, URL that will be opened after user taps on the text
    /// </summary>
    public string? UrlOrNull { get; } = urlOrNull;

    /// <summary>
    /// Optional. For <see cref="MessageEntityType.TextMention"/> only, the mentioned user
    /// </summary>
    public TelegramUser? FromUserOrNull { get; } = fromUserOrNull;
}