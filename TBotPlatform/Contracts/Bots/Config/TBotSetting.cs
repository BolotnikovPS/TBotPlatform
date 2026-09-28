#nullable enable

using Newtonsoft.Json;
using TBotPlatform.Contracts.Abstractions.Builder;
using Telegram.Bot.Types.Enums;

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
    /// Время ожидания между получением новых сообщений от telegram.
    /// Не используется при long polling: получение обновлений выполняет Telegram.Bot
    /// (удержание запроса GetUpdates регулируется <see cref="RequestTimeout" />).
    /// Параметр оставлен для обратной совместимости
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int HostWaitMilliSecond { get; set; } = 1000;

    /// <summary>
    /// Базовый URL локального Bot API Server (например, http://localhost:8081).
    /// Если задан, клиент работает с локальным сервером вместо https://api.telegram.org
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? BaseUrl { get; init; }

    /// <summary>
    /// Использовать тестовое окружение Bot API (…/bot{token}/test)
    /// </summary>
    public bool UseTestEnvironment { get; init; }

    /// <summary>
    /// Количество автоматических повторов запроса при ответе 429 "Too Many Requests".
    /// Если не задано, используется значение по умолчанию Telegram.Bot
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? RetryCount { get; init; }

    /// <summary>
    /// Порог RetryAfter (в секундах), при котором выполняется автоматический повтор запроса.
    /// Если не задано, используется значение по умолчанию Telegram.Bot
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? RetryThreshold { get; init; }

    /// <summary>
    /// Таймаут HTTP-запросов к Telegram. Для long polling это же значение используется как время удержания запроса GetUpdates.
    /// Если не задано, используется значение по умолчанию HttpClient (100 секунд)
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TimeSpan? RequestTimeout { get; init; }

    /// <summary>
    /// URL вебхука. Если задан, бот работает в режиме webhook вместо long polling.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? WebhookUrl { get; init; }

    /// <summary>
    /// Секретный токен для проверки входящих запросов webhook (заголовок X-Telegram-Bot-Api-Secret-Token).
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? WebhookSecretToken { get; init; }

    /// <summary>
    /// Настройки политик для работы с telegram
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TBotSettingHttpPolicy HttpPolicy { get; set; } = new();

    /// <summary>
    /// Режим разметки исходящих сообщений
    /// </summary>
    public ParseMode ParseMode { get; init; } = ParseMode.Html;

    /// <summary>
    /// Текст пользователю при необработанной ошибке состояния
    /// </summary>
    public string StateErrorText { get; init; } = "🆘 Произошла ошибка при обработке запроса.";

    /// <summary>
    /// Писать тела HTTP-запросов и полей Telegram API в лог. По умолчанию выключено.
    /// </summary>
    public bool VerboseLog { get; init; }
}
