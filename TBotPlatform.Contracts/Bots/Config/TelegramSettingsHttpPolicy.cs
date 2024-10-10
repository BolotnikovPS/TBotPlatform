#nullable enable
using System.Net;
using Newtonsoft.Json;

namespace TBotPlatform.Contracts.Bots.Config;

public class TelegramSettingsHttpPolicy
{
    /// <summary>
    /// <see cref="HttpStatusCode"/> по которым требуется выполнить повторный запрос в telegram
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int[]? BadStatuses { get; set; }

    /// <summary>
    /// Число повторений
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Интервал между повторениями
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int RetryMilliSecondInterval { get; set; } = 1000;
}