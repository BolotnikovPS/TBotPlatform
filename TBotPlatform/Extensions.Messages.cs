#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common;

public static partial class Extensions
{
    /// <summary>
    /// Checks whether a photo is present
    /// </summary>
    /// <param name="callbackQuery">The message</param>
    /// <returns></returns>
    public static bool WithPhoto(this CallbackQuery? callbackQuery) => callbackQuery?.Message?.Type == MessageType.Photo;

    /// <summary>
    /// Checks if an image is present, even if it's in a document
    /// </summary>
    /// <param name="message">The message</param>
    /// <returns></returns>
    public static bool WithImage(this Message? message) => message.IsNotNull() && (message?.Document?.MimeType?.Contains("image") == true || message!.Photo.IsNotNull());

    /// <summary>
    /// Checks if a document is present, excluding photos
    /// </summary>
    /// <param name="message">The message</param>
    /// <returns></returns>
    public static bool WithDocument(this Message? message) => message.IsNotNull() && !message.WithImage() && message!.Document.IsNotNull();

    /// <summary>
    /// Checks whether the message is a forward
    /// </summary>
    /// <param name="message">The message</param>
    /// <returns></returns>
    public static bool IsForwardMessage(this Message? message) => message.IsNotNull() && message!.ForwardOrigin.IsNotNull();

    /// <summary>
    /// Gets the message text
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="text">Message text</param>
    /// <returns></returns>
    public static bool TryGetText(this Message? message, out string? text)
    {
        if (message.IsNull())
        {
            text = null;
            return false;
        }

        text = message.WithImage() ? message!.Caption : message!.Text;
        return text != null;
    }

    /// <summary>
    /// Gets the message text
    /// </summary>
    /// <param name="callbackQuery">The message</param>
    /// <param name="text">Message text</param>
    /// <returns></returns>
    public static bool TryGetText(this CallbackQuery? callbackQuery, out string? text)
    {
        if (callbackQuery.IsNotNull()
            && callbackQuery!.Message.IsNotNull()
           )
        {
            return callbackQuery.Message.TryGetText(out text);
        }

        text = null;
        return false;
    }

    /// <summary>
    /// Gets user and chat data from an update
    /// </summary>
    /// <param name="update">Update from Telegram</param>
    /// <param name="telegramMessageUserData">Output data about the incoming request</param>
    /// <returns></returns>
    public static bool TryGetMessageUserData(this Update update, out TelegramMessageUserData? telegramMessageUserData)
    {
        telegramMessageUserData = update.Type switch
        {
            UpdateType.Message => new(update.Message?.From, update.Message?.Chat),
            UpdateType.GuestMessage => new(update.GuestMessage?.From, update.GuestMessage?.Chat),
            UpdateType.ManagedBot => new(update.ManagedBot?.User, chatOrNull: null),
            UpdateType.Subscription => new(update.Subscription?.User, chatOrNull: null),
            UpdateType.StoppedMessageGeneration => new(userOrNull: null, update.StoppedMessageGeneration?.Chat),
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
            UpdateType.MessageReactionCount => new(userOrNull: null, update.MessageReactionCount?.Chat),
            UpdateType.ChatBoost => new(userOrNull: null, update.ChatBoost?.Chat),
            UpdateType.RemovedChatBoost => new(userOrNull: null, update.RemovedChatBoost?.Chat),
            UpdateType.BusinessConnection => new(update.BusinessConnection?.User, chatOrNull: null),
            UpdateType.BusinessMessage => new(update.BusinessMessage?.From, update.BusinessMessage?.Chat),
            UpdateType.EditedBusinessMessage => new(update.EditedBusinessMessage?.From, update.EditedBusinessMessage?.Chat),
            UpdateType.DeletedBusinessMessages => new(userOrNull: null, update.DeletedBusinessMessages?.Chat),
            UpdateType.PurchasedPaidMedia => new(update.PurchasedPaidMedia?.From, chatOrNull: null),
            _ => null,
        };

        return !telegramMessageUserData.IsNull();
    }
}