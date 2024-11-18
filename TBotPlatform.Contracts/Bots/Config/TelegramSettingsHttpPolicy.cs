#nullable enable
using Newtonsoft.Json;
using System.Net;

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

    /// <summary>
    /// Интервал между отправкой запросов в телеграм
    /// У telegram ограничение, не более 30 обращений в 1 секунду.
    /// Но часто при установке минимального значения в 1 секунду возникают проблемы взаимодействия, ответ с <see cref="HttpStatusCode.TooManyRequests"/>.
    /// Для более точной настройки взаимодействия, подберите наиболее подходящий для вас интервал запросов
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int TelegramRequestMilliSecondInterval { get; set; } = 1000;
}