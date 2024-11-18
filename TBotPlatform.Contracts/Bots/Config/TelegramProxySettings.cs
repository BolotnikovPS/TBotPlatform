#nullable enable
using Newtonsoft.Json;
using TBotPlatform.Contracts.Abstractions.Factories.Proxies;

namespace TBotPlatform.Contracts.Bots.Config;

public class TelegramProxySettings
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
    /// Идентификатор бизнес аккаунта
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? BusinessConnectionId { get; init; }

    /// <summary>
    /// Префикс имя бота. Требуется в случае работы с состояниями <see cref="IStateProxyFactory"/>
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? BotPrefixName { get; init; }
}