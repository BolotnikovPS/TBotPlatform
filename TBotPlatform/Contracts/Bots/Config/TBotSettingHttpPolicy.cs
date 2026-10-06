#nullable enable
using Newtonsoft.Json;
using System.Net;

namespace TBotPlatform.Contracts.Bots.Config;

public class TBotSettingHttpPolicy
{
    /// <summary>
    /// The <see cref="HttpStatusCode"/> values that require a retry request to Telegram
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int[]? BadStatuses { get; set; }

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Interval between retry attempts
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int RetryMilliSecondInterval { get; set; } = 1000;

    /// <summary>
    /// Interval between requests sent to Telegram
    /// Telegram has a limit of no more than 30 requests per second.
    /// However, when the minimum value of 1 second is set, interaction problems often occur,
    /// with responses of <see cref="HttpStatusCode.TooManyRequests"/>.
    /// For more precise interaction tuning, choose the request interval that suits you best
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int TelegramRequestMilliSecondInterval { get; set; } = 1000;
}