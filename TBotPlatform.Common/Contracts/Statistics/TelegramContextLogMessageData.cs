#nullable enable
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contracts.Statistics;

public class TelegramContextLogMessageData
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long FromChatId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Message { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? MessageId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public InlineKeyboardMarkup? InlineKeyboardMarkup { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IReplyMarkup? ReplyMarkup { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Caption { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ChatAction? ChatAction { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public InputFile? InputFile { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? DisableNotification { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int ReplyToMessageId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool AllowSendingWithoutReply { get; set; }
}