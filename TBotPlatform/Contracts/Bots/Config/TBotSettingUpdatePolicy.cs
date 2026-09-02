#nullable enable

using Newtonsoft.Json;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts.Bots.Config;

public class TBotSettingUpdatePolicy
{
    /// <summary>
    /// Типы обновлений которые будет получать бот
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public UpdateType[]? Type { get; set; }

    /// <summary>
    /// Размер пакетов с обновлениями от telegram
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? Capacity { get; set; }
}
