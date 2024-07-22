#nullable enable
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Statistics;

public class StatisticMessage
{
    public required long ChatId { get; set; }

    public string? Message { get; set; }

    public int? MessageId { get; set; }

    public InlineKeyboardMarkup? InlineKeyboardMarkup { get; set; }

    public IReplyMarkup? ReplyMarkup { get; set; }

    public string? Caption { get; set; }

    public ChatAction? ChatAction { get; set; }

    public InputFile? InputFile { get; set; }

    public required string OperationType { get; set; }
}