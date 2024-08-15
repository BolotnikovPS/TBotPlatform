using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum EChatMessageType
{
    None = 0,
    Message,
    ForwardMessage,
    ToReplyMessage,
}