using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TBotPlatform.Contracts.Bots.Chats;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum EChatMemberStatus
{
    None = 0,

    /// <summary>
    /// Creator of the <see cref="TelegramChat"/>
    /// </summary>
    Creator,

    /// <summary>
    /// Administrator of the <see cref="TelegramChat"/>
    /// </summary>
    Administrator,

    /// <summary>
    /// Normal member of the <see cref="TelegramChat"/>
    /// </summary>
    Member,

    /// <summary>
    /// A <see cref="TelegramChat"/> who left the <see cref="TelegramChat"/>
    /// </summary>
    Left,

    /// <summary>
    /// A <see cref="TelegramChat"/> who was kicked from the <see cref="TelegramChat"/>
    /// </summary>
    Kicked,

    /// <summary>
    /// A <see cref="TelegramChat"/> who is restricted in the <see cref="TelegramChat"/>
    /// </summary>
    Restricted,
}