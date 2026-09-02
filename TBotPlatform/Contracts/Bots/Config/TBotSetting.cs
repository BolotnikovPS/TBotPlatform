#nullable enable

using Newtonsoft.Json;
using TBotPlatform.Contracts.Abstractions.Builder;

namespace TBotPlatform.Contracts.Bots.Config;

public class TBotSetting
{
    /// <summary>
    /// Наименование бота
    /// </summary>
    public required string BotName { get; set; }

    /// <summary>
    /// Токен бота
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// Конфиденциальные настройки бота по отправке сообщений
    /// </summary>
    public bool ProtectContent { get; init; }

    /// <summary>
    /// Информация о механимзе получения обновлений от telegram
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TBotSettingUpdatePolicy? UpdatePolicy { get; set; }

    /// <summary>
    /// Время ожидания между получением новых сообщений от telegram. Заполнить в случае использования <see cref="IBotPlatformBuilder.AddHostedService" />
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int HostWaitMilliSecond { get; set; } = 1000;

    /// <summary>
    /// Настройки политик для работы с telegram
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TBotSettingHttpPolicy HttpPolicy { get; set; } = new();
}
