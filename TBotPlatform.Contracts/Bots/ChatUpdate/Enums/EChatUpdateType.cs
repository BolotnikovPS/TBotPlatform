namespace TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

public enum EChatUpdateType
{
    None = 0,
    Message,
    ForwardMessage,
    ToReplyMessage,
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