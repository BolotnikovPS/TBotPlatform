using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum EChatUpdateType
{
    None = 0,
    Message,
    InlineQuery,
    ChosenInlineResult,
    CallbackQuery,
    EditedMessage,
    ChannelPost,
    EditedChannelPost,
    ShippingQuery,
    PreCheckoutQuery,
    Poll,
    PollAnswer,
    MyChatMember,
    ChatMember,
    ChatJoinRequest,
}