#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts;

public static partial class Extensions
{
    public static bool WithImage(this CallbackQuery? callbackQuery) => callbackQuery?.Message?.Type == MessageType.Photo;

    public static bool TryGetMessageUserData(this Update update, out TelegramMessageUserData? telegramMessageUserData)
    {
        telegramMessageUserData = update.Type switch
        {
            UpdateType.Message => new(update.Message?.From, update.Message?.Chat),
            UpdateType.InlineQuery => new(update.InlineQuery?.From, chatOrNull: null),
            UpdateType.ChosenInlineResult => new(update.ChosenInlineResult?.From, chatOrNull: null),
            UpdateType.CallbackQuery => new(update.CallbackQuery?.From, update.CallbackQuery?.Message?.Chat),
            UpdateType.EditedMessage => new(update.EditedMessage?.From, update.EditedMessage?.Chat),
            UpdateType.ChannelPost => new(update.ChannelPost?.From, update.ChannelPost?.Chat),
            UpdateType.EditedChannelPost => new(update.EditedChannelPost?.From, update.EditedChannelPost?.Chat),
            UpdateType.ShippingQuery => new(update.ShippingQuery?.From, chatOrNull: null),
            UpdateType.PreCheckoutQuery => new(update.PreCheckoutQuery?.From, chatOrNull: null),
            UpdateType.PollAnswer => new(update.PollAnswer?.User, update.PollAnswer?.VoterChat),
            UpdateType.MyChatMember => new(update.MyChatMember?.From, update.MyChatMember?.Chat),
            UpdateType.ChatMember => new(update.ChatMember?.From, update.ChatMember?.Chat),
            UpdateType.ChatJoinRequest => new(update.ChatJoinRequest?.From, update.ChatJoinRequest?.Chat),
            UpdateType.MessageReaction => new(update.MessageReaction?.User, update.MessageReaction?.Chat),
            UpdateType.MessageReactionCount => new(null, update.MessageReactionCount?.Chat),
            UpdateType.ChatBoost => new(null, update.ChatBoost?.Chat),
            UpdateType.RemovedChatBoost => new(null, update.RemovedChatBoost?.Chat),
            UpdateType.BusinessConnection => new(update.BusinessConnection?.User, chatOrNull: null),
            UpdateType.BusinessMessage => new(update.BusinessMessage?.From, update.BusinessMessage?.Chat),
            UpdateType.EditedBusinessMessage => new(update.EditedBusinessMessage?.From, update.EditedBusinessMessage?.Chat),
            UpdateType.DeletedBusinessMessages => new(null, update.DeletedBusinessMessages?.Chat),
            UpdateType.PurchasedPaidMedia => new(update.PurchasedPaidMedia?.From, null),
            _ => null,
        };

        return !telegramMessageUserData.IsNull();
    }
}