#nullable enable

using Newtonsoft.Json;
using TBotPlatform.Contracts.Abstractions.Builder;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts.Bots.Config;

public class TBotSetting
{
    /// <summary>
    /// Bot name
    /// </summary>
    public required string BotName { get; set; }

    /// <summary>
    /// Bot token
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// Confidential bot settings for sending messages
    /// </summary>
    public bool ProtectContent { get; init; }

    /// <summary>
    /// Information about the mechanism for receiving updates from Telegram
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TBotSettingUpdatePolicy? UpdatePolicy { get; set; }

    /// <summary>
    /// Wait time between receiving new messages from Telegram.
    /// Not used for long polling: Telegram Bot performs GetUpdates requests
    /// (the hold time of GetUpdates request is governed by <see cref="RequestTimeout" />).
    /// Parameter kept for backward compatibility
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int HostWaitMilliSecond { get; set; } = 1000;

    /// <summary>
    /// Base URL of the local Bot API Server (for example, http://localhost:8081).
    /// If specified, the client works with a local server instead of https://api.telegram.org
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? BaseUrl { get; init; }

    /// <summary>
    /// SOCKS5 proxy settings. If specified, all Telegram API traffic is routed through the proxy.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TBotSettingProxy? Proxy { get; init; }

    /// <summary>
    /// Use the test environment of Bot API (…/bot{token}/test)
    /// </summary>
    public bool UseTestEnvironment { get; init; }

    /// <summary>
    /// Number of automatic retries on 429 "Too Many Requests" response.
    /// If not specified, the default value of Telegram.Bot is used
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? RetryCount { get; init; }

    /// <summary>
    /// RetryAfter threshold (in seconds), at which automatic request retry is executed.
    /// If not specified, the default value of Telegram.Bot is used
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? RetryThreshold { get; init; }

    /// <summary>
    /// HTTP request timeout to Telegram. For long polling, this same value is used as the GetUpdates request hold time.
    /// If not specified, the default value of HttpClient (100 seconds) is used
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TimeSpan? RequestTimeout { get; init; }

    /// <summary>
    /// Webhook URL. If specified, the bot works in webhook mode instead of long polling.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? WebhookUrl { get; init; }

    /// <summary>
    /// Secret token for verifying incoming webhook requests (header X-Telegram-Bot-Api-Secret-Token).
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? WebhookSecretToken { get; init; }

    /// <summary>
    /// Policies settings for working with Telegram
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TBotSettingHttpPolicy HttpPolicy { get; set; } = new();

    /// <summary>
    /// Message formatting mode for outgoing messages
    /// </summary>
    public ParseMode ParseMode { get; init; } = ParseMode.Html;

    /// <summary>
    /// Text for the user when an unhandled state error occurs
    /// </summary>
    public string StateErrorText { get; init; } = "🆘 An error occurred while processing the request.";

    /// <summary>
    /// Log HTTP request bodies and Telegram API fields. Disabled by default.
    /// </summary>
    public bool VerboseLog { get; init; }
}
