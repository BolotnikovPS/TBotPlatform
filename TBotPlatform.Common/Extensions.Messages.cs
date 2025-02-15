#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common;

public static partial class Extensions
{
    /// <summary>
    /// Проверяет наличие фотографии
    /// </summary>
    /// <param name="callbackQuery">Сообщение</param>
    /// <returns></returns>
    public static bool WithPhoto(this CallbackQuery? callbackQuery) => callbackQuery?.Message?.Type == MessageType.Photo;

    /// <summary>
    /// Проверяет наличие изображение, даже если оно в документе
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns></returns>
    public static bool WithImage(this Message? message) => message.IsNotNull() && (message?.Document?.MimeType?.Contains("image") == true || message!.Photo.IsNotNull());

    /// <summary>
    /// Проверяет наличие документа, кроме фото
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns></returns>
    public static bool WithDocument(this Message? message) => !message.WithImage() && message!.Document.IsNotNull();

    /// <summary>
    /// Проверяет тип сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns></returns>
    public static bool IsForwardMessage(this Message? message) => message.IsNotNull() && message!.ForwardOrigin.IsNotNull();

    /// <summary>
    /// Получает текст сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="text">Текст сообщения</param>
    /// <returns></returns>
    public static bool TryGetText(this Message? message, out string? text)
    {
        if (message.IsNull())
        {
            text = null;
            return false;
        }

        text = message.WithImage() ? message!.Caption : message!.Text;
        return true;
    }

    /// <summary>
    /// Получает текст сообщения
    /// </summary>
    /// <param name="callbackQuery">Сообщение</param>
    /// <param name="text">Текст сообщения</param>
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
    /// Получает данные о пользователе и чате
    /// </summary>
    /// <param name="update">Запрос с telegram</param>
    /// <param name="telegramMessageUserData">Выходные данные о входящем запросе</param>
    /// <returns></returns>
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