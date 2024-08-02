using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    public Task PinChatMessageAsync(long chatId, int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(PinChatMessageAsync),
            ChatId = chatId,
            MessageId = messageId,
        };

        var task = _botClient.PinChatMessageAsync(chatId, messageId, disableNotification, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task UnpinChatMessageAsync(long chatId, int messageId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(UnpinChatMessageAsync),
            ChatId = chatId,
            MessageId = messageId,
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
        };

        var task = _botClient.UnpinAllChatMessages(chatId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}