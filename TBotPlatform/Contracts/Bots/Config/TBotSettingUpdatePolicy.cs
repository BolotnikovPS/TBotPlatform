#nullable enable

using Newtonsoft.Json;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts.Bots.Config;

public class TBotSettingUpdatePolicy
{
    /// <summary>
    /// Update types that the bot will receive
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public UpdateType[]? Type { get; set; }

    /// <summary>
    /// Size of update batches received from Telegram
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? Capacity { get; set; }
}
