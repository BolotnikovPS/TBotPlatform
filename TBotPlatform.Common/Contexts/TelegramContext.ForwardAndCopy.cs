using TBotPlatform.Common.Contracts;
using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    public Task<Message> ForwardMessageAsync(long chatId, long fromChatId, int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            FromChatId = fromChatId,
            MessageId = messageId,
            DisableNotification = disableNotification,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(ForwardMessageAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };

        var task = _botClient.ForwardMessageAsync(
            chatId,
            fromChatId,
            messageId,
            protectContent: telegramSettings.ProtectContent,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<MessageId> CopyMessageAsync(
        long chatId,
        long fromChatId,
        int messageId,
        string caption,
        int replyToMessageId,
        bool allowSendingWithoutReply,
        IReplyMarkup replyMarkup,
        bool disableNotification,
        CancellationToken cancellationToken
        )
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            FromChatId = fromChatId,
            MessageId = messageId,
            Caption = caption,
            ReplyToMessageId = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup = replyMarkup,
            DisableNotification = disableNotification,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(CopyMessageAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };

        var task = _botClient.CopyMessageAsync(
            chatId,
            fromChatId,
            messageId,
            messageThreadId: null,
            caption,
            ParseMode,
            captionEntities: null,
            disableNotification,
            telegramSettings.ProtectContent,
            replyToMessageId,
            allowSendingWithoutReply,
            replyMarkup,
            cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}