using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum EChatMemberStatus
{
    None = 0,

    /// <summary>
    /// Creator of the <see cref="Chat"/>
    /// </summary>
    Creator,

    /// <summary>
    /// Administrator of the <see cref="Chat"/>
    /// </summary>
    Administrator,

    /// <summary>
    /// Normal member of the <see cref="Chat"/>
    /// </summary>
    Member,

    /// <summary>
    /// A <see cref="User"/> who left the <see cref="Chat"/>
    /// </summary>
    Left,

    /// <summary>
    /// A <see cref="User"/> who was kicked from the <see cref="Chat"/>
    /// </summary>
    Kicked,

    /// <summary>
    /// A <see cref="User"/> who is restricted in the <see cref="Chat"/>
    /// </summary>
    Restricted,
}