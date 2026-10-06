#nullable enable
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Bots.Markups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

public interface IStateContextMinimal : IAsyncDisposable
{
    /// <summary>
    /// Gets the OperationGuid of the current Telegram request pools
    /// </summary>
    Guid CurrentOperation { get; }

    /// <summary>
    /// Gets the context for working with Telegram directly
    /// </summary>
    ITelegramContext TelegramContext { get; }

    /// <summary>
    /// Sends documents to the chat
    /// </summary>
    /// <param name="documentData">Document file</param>
    /// <param name="caption"></param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendDocument(FileDataBase documentData, string? caption, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Sends documents to the chat
    /// </summary>
    /// <param name="documentData">Document file</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendDocument(FileDataBase documentData, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Sends documents to the chat
    /// </summary>
    /// <param name="documentData">Document file</param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendDocument(FileDataBase documentData, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a photo to the chat
    /// </summary>
    /// <param name="documentData">Document file</param>
    /// <param name="caption"></param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendPhoto(FileDataBase documentData, string? caption, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a photo to the chat
    /// </summary>
    /// <param name="documentData">Document file</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendPhoto(FileDataBase documentData, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a photo to the chat
    /// </summary>
    /// <param name="documentData">Document file</param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendPhoto(FileDataBase documentData, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a media group (a group of files) to the chat. Telegram accepts 2 to 10 files per media group
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="mediaDatas">Files of the media group. Each file is sent as an image</param>
    /// <param name="disableNotification">Disable user notification</param>
    Task<Message[]> SendMediaGroup(IReadOnlyList<FileDataBase> mediaDatas, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a media group (a group of files) to the chat. Telegram accepts 2 to 10 files per media group
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="mediaDatas">Files of the media group. Each file is sent as an image</param>
    Task<Message[]> SendMediaGroup(IReadOnlyList<FileDataBase> mediaDatas, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a media group (a group of files) to the chat. Telegram accepts 2 to 10 files per media group
    /// </summary>
    /// <param name="mediaDatas">Album files</param>
    /// <param name="disableNotification">Disable the notification for the user</param>
    /// <param name="cancellationToken"></param>
    /// <param name="mediaGroupType">Type of files to send: photo, video, or document</param>
    Task<Message[]> SendMediaGroup(
        IReadOnlyList<FileDataBase> mediaDatas,
        MediaGroupType mediaGroupType,
        bool disableNotification,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Sends a media group (a group of files) to the chat. Telegram accepts 2 to 10 files per media group
    /// </summary>
    /// <param name="mediaDatas">Album files</param>
    /// <param name="cancellationToken"></param>
    /// <param name="mediaGroupType">Type of files to send: photo, video, or document</param>
    Task<Message[]> SendMediaGroup(
        IReadOnlyList<FileDataBase> mediaDatas,
        MediaGroupType mediaGroupType,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Updates only the buttons of the message that triggered the request (without deleting and resending it)
    /// </summary>
    /// <param name="inlineMarkupMassiveList">Buttons</param>
    /// <param name="cancellationToken"></param>
    Task<Message> UpdateInlineMarkup(InlineMarkupMassiveList inlineMarkupMassiveList, CancellationToken cancellationToken);

    /// <summary>
    /// Sends or updates a message with the attached buttons in the chat
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="inlineMarkupList"></param>
    /// <param name="photoData">Image file</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendOrUpdateTextMessage(
        string text,
        InlineMarkupList inlineMarkupList,
        FileDataBase photoData,
        bool disableNotification,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Sends or updates a message with the attached buttons in the chat
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="inlineMarkupList"></param>
    /// <param name="photoData">Image file</param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupList inlineMarkupList, FileDataBase photoData, CancellationToken cancellationToken);

    /// <summary>
    /// Sends or updates a message with the attached buttons in the chat
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="inlineMarkupList"></param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupList inlineMarkupList, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Sends or updates a message with the attached buttons in the chat
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="inlineMarkupList"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupList inlineMarkupList, CancellationToken cancellationToken);

    /// <summary>
    /// Sends or updates a message with the attached buttons in the chat
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="inlineMarkupMassiveList"></param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupMassiveList inlineMarkupMassiveList, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Sends or updates a message with the attached buttons in the chat
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="inlineMarkupMassiveList"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupMassiveList inlineMarkupMassiveList, CancellationToken cancellationToken);

    /// <summary>
    /// Sends or updates a message with the attached buttons in the chat
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendOrUpdateTextMessage(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Sends or updates a message with the attached buttons in the chat
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendOrUpdateTextMessage(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a message to the chat as a reply
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendTextMessageWithReply(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a message to the chat as a reply
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendTextMessageWithReply(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a message to the chat as a reply
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendTextMessage(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a message to the chat as a reply
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendTextMessage(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a message with extended parameters: reply to a message, link preview settings, message effect
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="cancellationToken"></param>
    /// <param name="disableNotification">Disable user notification</param>
    /// <param name="replyParameters">Reply parameters for the message</param>
    /// <param name="linkPreviewOptions">Link preview display settings</param>
    /// <param name="messageEffectId">Unique identifier of the message effect (only for private chats)</param>
    Task<Message> SendTextMessage(
        string text,
        bool disableNotification,
        ReplyParameters? replyParameters,
        LinkPreviewOptions? linkPreviewOptions,
        string? messageEffectId,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Sends a message with explicitly specified markup entities instead of parse mode.
    /// Allows resending formatting without loss: tg-emoji, custom emoji, spoilers, quotes
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="cancellationToken"></param>
    /// <param name="entities">Text markup entities</param>
    /// <param name="disableNotification">Disable user notification</param>
    Task<Message> SendTextMessageWithEntities(
        string text,
        IReadOnlyList<MessageEntity> entities,
        bool disableNotification,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Sends a message with explicitly specified markup entities instead of parse mode
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="cancellationToken"></param>
    /// <param name="entities">Text markup entities</param>
    Task<Message> SendTextMessageWithEntities(
        string text,
        IReadOnlyList<MessageEntity> entities,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Sends a long text
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    Task SendLongTextMessage(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a long text
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="cancellationToken"></param>
    Task SendLongTextMessage(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a chat action
    /// </summary>
    /// <param name="chatAction">Chat action</param>
    /// <param name="cancellationToken"></param>
    Task SendChatAction(ChatAction chatAction, CancellationToken cancellationToken);

    /// <summary>
    /// Removes main buttons in the chat
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="cancellationToken"></param>
    Task<Message> RemoveMarkup(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the main buttons in the chat
    /// </summary>
    /// <param name="replyMarkup"></param>
    /// <param name="cancellationToken"></param>
    Task<Message> UpdateMainButtons(MainButtonMassiveList replyMarkup, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the main buttons in the chat
    /// </summary>
    /// <param name="replyMarkup"></param>
    /// <param name="text">Message text</param>
    /// <param name="cancellationToken"></param>
    Task<Message> UpdateMainButtons(MainButtonMassiveList replyMarkup, string text, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the message, replaces the text and removes the buttons
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="cancellationToken"></param>
    Task UpdateMarkupTextAndDropButton(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the message, replaces the text and removes the buttons
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task UpdateMarkupTextAndDropButton(CancellationToken cancellationToken);

    /// <summary>
    /// Removes the message with a button from which the request came
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <param name="cancellationToken"></param>
    Task RemoveMessage(int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Forwards a message
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <param name="cancellationToken"></param>
    /// <param name="fromChatId">Id of the chat where the data is taken from</param>
    /// <param name="disableNotification">Disable user notification</param>
    Task<Message> ForwardMessage(long fromChatId, int messageId, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Forwards a message
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <param name="cancellationToken"></param>
    /// <param name="fromChatId">Id of the chat where the data is taken from</param>
    Task<Message> ForwardMessage(long fromChatId, int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Copies a message
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <param name="replyMarkup">Buttons</param>
    /// <param name="cancellationToken"></param>
    /// <param name="fromChatId">Id of the chat where the data is taken from</param>
    /// <param name="caption">Caption/text for the message</param>
    /// <param name="replyToMessageId">Id of the message to reply to</param>
    /// <param name="disableNotification">Disable user notification</param>
    Task<int> CopyMessage(
        long fromChatId,
        int messageId,
        string caption,
        int replyToMessageId,
        ReplyMarkup replyMarkup,
        bool disableNotification,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Pins the message
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <param name="disableNotification">Disable the notification for the user</param>
    /// <param name="cancellationToken"></param>
    Task PinChatMessage(int messageId, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Pins the message
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <param name="cancellationToken"></param>
    Task PinChatMessage(int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Unpins the message
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <param name="cancellationToken"></param>
    Task UnpinChatMessage(int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Unpins all messages
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task UnpinAllChatMessages(CancellationToken cancellationToken);

    /// <summary>
    /// Counts the chat members
    /// </summary>
    /// <param name="chatIdToCheck"></param>
    /// <param name="cancellationToken"></param>
    Task<int> GetChatMemberCount(long chatIdToCheck, CancellationToken cancellationToken);

    /// <summary>
    /// Gets information for a user about the chat
    /// </summary>
    /// <param name="chatIdToCheck"></param>
    /// <param name="userIdToCheck"></param>
    /// <param name="cancellationToken"></param>
    Task<ChatMember> GetChatMember(long chatIdToCheck, long userIdToCheck, CancellationToken cancellationToken);

    /// <summary>
    /// Gets information about the list of chat administrators
    /// </summary>
    /// <param name="chatIdToCheck"></param>
    /// <param name="cancellationToken"></param>
    Task<List<ChatMember>> GetChatAdministrators(long chatIdToCheck, CancellationToken cancellationToken);

    /// <summary>
    /// Sends an answer to the client for callback queries
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="text">Notification text</param>
    /// <param name="showAlert">If true, the client will show an alert instead of a notification at the top of the chat screen</param>
    /// <param name="url">URL that will be opened by the user's client. If you created an <c>InlineMarkupCallBackGame</c> and accepted the terms via <a href="https://t.me/botfather">@BotFather</a>, provide the URL that opens your game.
    /// Otherwise, you can use links such as <c>t.me/your_bot?start=XXXX</c> that open your bot with a parameter.</param>
    /// <param name="cacheTime">Maximum time in seconds during which the callback query result may be shown on the client side</param>
    Task AnswerCallbackQuery(
        string text,
        bool showAlert,
        string url,
        int? cacheTime,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Sends an answer to the user's inline query
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="inlineQueryId">Inline query identifier</param>
    /// <param name="results">Query results, no more than 50</param>
    /// <param name="cacheTime">Server-side cache time for the result in seconds, default 300</param>
    /// <param name="nextOffset">Offset for the next page of results</param>
    Task AnswerInlineQuery(
        string inlineQueryId,
        IReadOnlyList<InlineQueryResult> results,
        int? cacheTime,
        string? nextOffset,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Sends an answer to a web app query
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="webAppQueryId">Identifier of the query from <c>WebAppQuery</c></param>
    /// <param name="result">Result that will be shown to the user</param>
    Task<SentWebAppMessage> AnswerWebAppQuery(string webAppQueryId, InlineQueryResult result, CancellationToken cancellationToken);

    /// <summary>
    /// Confirms or rejects the payment. The answer must be sent within 10 seconds
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="preCheckoutQueryId">Query identifier</param>
    /// <param name="errorMessage">Error text if the payment cannot be processed. An empty value confirms the payment</param>
    Task AnswerPreCheckoutQuery(string preCheckoutQueryId, string? errorMessage, CancellationToken cancellationToken);

    /// <summary>
    /// Sends delivery options or a reason why delivery is impossible
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="shippingQueryId">Query identifier</param>
    /// <param name="shippingOptions">Delivery options, if delivery is possible</param>
    /// <param name="errorMessage">Error text if delivery is impossible</param>
    Task AnswerShippingQuery(
        string shippingQueryId,
        IReadOnlyList<ShippingOption>? shippingOptions,
        string? errorMessage,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Sends an invoice for payment to the chat
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="title">Product name, 1-32 symbols</param>
    /// <param name="description">Product description, 1-255 symbols</param>
    /// <param name="payload">Internal bot payload, 1-128 bytes</param>
    /// <param name="currency">Three-letter ISO 4217 currency code, use <c>XTR</c> for Telegram Stars</param>
    /// <param name="prices">Price components</param>
    /// <param name="providerToken">Payment provider token, empty string for Telegram Stars</param>
    /// <param name="providerData">Payment provider data in JSON format</param>
    Task<Message> SendInvoice(
        string title,
        string description,
        string payload,
        string currency,
        IReadOnlyList<LabeledPrice> prices,
        string? providerToken,
        string? providerData,
        CancellationToken cancellationToken
        );
}