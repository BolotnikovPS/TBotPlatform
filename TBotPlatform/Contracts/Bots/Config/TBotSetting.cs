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
    /// Время ожидания между получением новых сообщений от telegram. Заполнить в случае использования <see cref="IBotPlatformBuilder.AddHostedService" />
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int HostWaitMilliSecond { get; set; } = 1000;

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
