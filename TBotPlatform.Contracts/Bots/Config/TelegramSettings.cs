#nullable enable
using Newtonsoft.Json;
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

namespace TBotPlatform.Contracts.Bots.Config;

public class TelegramSettings
{
    /// <summary>
    /// Токен бота
    /// </summary>
    public string? Token { get; init; }

    /// <summary>
    /// Конфиденциальные настройки бота по отправке сообщений
    /// </summary>
    public bool ProtectContent { get; init; }

    /// <summary>
    /// Типы обновлений которые будет получать бот
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public EChatUpdateType[]? UpdateType { get; set; }

    /// <summary>
    /// Время ожидания между получением новых сообщений от telegram
    /// заполнить в случае использования HostedService
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int HostWaitMilliSecond { get; set; } = 1000;

    /// <summary>
    /// Настройки политик для работы с telegram
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TelegramSettingsHttpPolicy HttpPolicy { get; set; } = new();
}