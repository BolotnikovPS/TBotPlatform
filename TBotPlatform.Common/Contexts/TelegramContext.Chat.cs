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
            MessageBody = new(),
        };

        var task = _botClient.GetChatMemberCount(chatId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<ChatMember> GetChatMemberAsync(long chatId, long userId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(GetChatMemberAsync),
            ChatId = chatId,
            MessageBody = new(),
        };

        var task = _botClient.GetChatMember(chatId, userId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<ChatMember[]> GetChatAdministratorsAsync(long chatId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(GetChatAdministratorsAsync),
            ChatId = chatId,
            MessageBody = new(),
        };

        var task = _botClient.GetChatAdministrators(chatId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<ChatFullInfo> GetChatAsync(long chatId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(GetChatAsync),
            ChatId = chatId,
            MessageBody = new(),
        };

        var task = _botClient.GetChat(chatId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task LeaveChatAsync(long chatId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(LeaveChatAsync),
            ChatId = chatId,
            MessageBody = new(),
        };

        var task = _botClient.LeaveChat(chatId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}