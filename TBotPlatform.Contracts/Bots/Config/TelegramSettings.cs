using Newtonsoft.Json;
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

namespace TBotPlatform.Contracts.Bots.Config;

public class TelegramSettings
{
    public string Token { get; init; }
    
    public bool ProtectContent { get; init; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public EChatUpdateType[] UpdateType { get; set; }
    
    /// <summary>
    /// Время ожидания между получением новых сообщений от telegram
    /// Заполняется в случае использования 
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int HostWaitMilliSecond { get; set; } = 1000;
}