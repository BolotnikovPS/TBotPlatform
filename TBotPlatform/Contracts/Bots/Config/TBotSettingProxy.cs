#nullable enable

using Newtonsoft.Json;

namespace TBotPlatform.Contracts.Bots.Config;

/// <summary>
/// SOCKS5 proxy settings. When specified on <see cref="TBotSetting.Proxy"/>,
/// all Telegram API traffic is routed through the proxy.
/// </summary>
public class TBotSettingProxy
{
    /// <summary>
    /// Proxy host (IP address or domain name)
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    /// Proxy port. Defaults to the standard SOCKS5 port 1080.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int Port { get; init; } = 1080;

    /// <summary>
    /// Username for SOCKS5 authentication (RFC 1929). Omit for an anonymous proxy.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Username { get; init; }

    /// <summary>
    /// Password for SOCKS5 authentication (RFC 1929). Omit for an anonymous proxy.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Password { get; init; }
}
