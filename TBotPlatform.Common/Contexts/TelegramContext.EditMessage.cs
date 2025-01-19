#nullable enable
using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    public Task<Message> EditMessageTextAsync(
        long chatId,
        int messageId,
        string text,
        InlineKeyboardMarkup? replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            MessageId = messageId,
            Message = text,
            InlineKeyboardMarkup = replyMarkup,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(EditMessageTextAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };

        var task = _botClient.EditMessageText(
            chatId,
            messageId,
            text,
            ParseMode,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> EditMessageTextAsync(long chatId, int messageId, string text, CancellationToken cancellationToken)
        => EditMessageTextAsync(chatId, messageId, text, replyMarkup: null, cancellationToken);

    public Task<Message> EditMessageCaptionAsync(long chatId, int messageId, string caption, InlineKeyboardMarkup? replyMarkup, CancellationToken cancellationToken)
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            MessageId = messageId,
            InlineKeyboardMarkup = replyMarkup,
            Caption = caption,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(EditMessageCaptionAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };

        var task = _botClient.EditMessageCaption(
            chatId,
            messageId,
            caption,
            ParseMode,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> EditMessageCaptionAsync(long chatId, int messageId, string caption, CancellationToken cancellationToken)
        => EditMessageCaptionAsync(chatId, messageId, caption, replyMarkup: null, cancellationToken);

    public Task<Message> EditMessageReplyMarkupAsync(
        long chatId,
        int messageId,
        InlineKeyboardMarkup replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            MessageId = messageId,
            InlineKeyboardMarkup = replyMarkup,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(EditMessageReplyMarkupAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };
        
        var task = _botClient.EditMessageReplyMarkup(
            chatId,
            messageId,
            replyMarkup,
            telegramSettings.Value?.BusinessConnectionId,
            cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}