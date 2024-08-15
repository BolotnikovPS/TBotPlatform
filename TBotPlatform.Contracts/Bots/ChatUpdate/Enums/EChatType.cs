using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum EChatType
{
    None = 0,

    /// <summary>
    /// Normal one to one
    /// </summary>
    Private,

    /// <summary>
    /// Normal group chat
    /// </summary>
    Group,

    /// <summary>
    /// A channel
    /// </summary>
    Channel,

    /// <summary>
    /// A supergroup
    /// </summary>
    Supergroup,

    /// <summary>
    /// “sender” for a private chat with the inline query sender
    /// </summary>
    Sender,
}