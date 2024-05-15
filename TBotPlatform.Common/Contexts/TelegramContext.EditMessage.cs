using TBotPlatform.Contracts.Statistics;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using System;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    public Task<Message> EditMessageTextAsync(
        long chatId,
        int messageId,
        string text,
        InlineKeyboardMarkup replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(EditMessageTextAsync),
            ChatId = chatId,
            MessageId = messageId,
            Message = text,
            InlineKeyboardMarkup = replyMarkup,
        };

        var task = _botClient.EditMessageTextAsync(
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
        => EditMessageTextAsync(chatId, messageId, text, null, cancellationToken);

    public Task<Message> EditMessageCaptionAsync(long chatId, int messageId, string caption, InlineKeyboardMarkup replyMarkup, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(EditMessageCaptionAsync),
            ChatId = chatId,
            MessageId = messageId,
            InlineKeyboardMarkup = replyMarkup,
            Caption = caption,
        };

        var task = _botClient.EditMessageCaptionAsync(
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
        => EditMessageCaptionAsync(chatId, messageId, caption, null, cancellationToken);

    public Task<Message> EditMessageReplyMarkupAsync(
        long chatId,
        int messageId,
        InlineKeyboardMarkup replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(EditMessageReplyMarkupAsync),
            ChatId = chatId,
            MessageId = messageId,
            InlineKeyboardMarkup = replyMarkup,
        };

        var task = _botClient.EditMessageReplyMarkupAsync(
            chatId,
            messageId,
            replyMarkup,
            cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}