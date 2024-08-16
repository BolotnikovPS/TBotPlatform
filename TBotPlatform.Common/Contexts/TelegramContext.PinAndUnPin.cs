using TBotPlatform.Common.Contracts.Statistics;
using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    public Task PinChatMessageAsync(long chatId, int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            MessageId = messageId,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(PinChatMessageAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };

        var task = _botClient.PinChatMessageAsync(chatId, messageId, disableNotification, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task UnpinChatMessageAsync(long chatId, int messageId, CancellationToken cancellationToken)
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            MessageId = messageId,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(UnpinChatMessageAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };

        var task = _botClient.UnpinChatMessageAsync(chatId, messageId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task UnpinAllChatMessages(long chatId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(UnpinAllChatMessages),
            ChatId = chatId,
            MessageBody = "",
        };

        var task = _botClient.UnpinAllChatMessages(chatId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}