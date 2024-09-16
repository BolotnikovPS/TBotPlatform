using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    public Task<int> GetChatMemberCountAsync(long chatId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(GetChatMemberCountAsync),
            ChatId = chatId,
            MessageBody = "",
        };

        var task = _botClient.GetChatMemberCountAsync(chatId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<ChatMember> GetChatMemberAsync(long chatId, long userId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(GetChatMemberAsync),
            ChatId = chatId,
            MessageBody = "",
        };

        var task = _botClient.GetChatMemberAsync(chatId, userId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<ChatMember[]> GetChatAdministratorsAsync(long chatId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(GetChatAdministratorsAsync),
            ChatId = chatId,
            MessageBody = "",
        };

        var task = _botClient.GetChatAdministratorsAsync(chatId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Chat> GetChatAsync(long chatId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(GetChatAsync),
            ChatId = chatId,
            MessageBody = "",
        };

        var task = _botClient.GetChatAsync(chatId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task LeaveChatAsync(long chatId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(LeaveChatAsync),
            ChatId = chatId,
            MessageBody = "",
        };

        var task = _botClient.LeaveChatAsync(chatId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}