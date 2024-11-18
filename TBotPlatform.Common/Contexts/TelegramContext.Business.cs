using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    public Task<BusinessConnection> GetBusinessConnectionAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(telegramSettings.BusinessConnectionId);

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(GetBusinessConnectionAsync),
            ChatId = 0,
            MessageBody = new(),
        };

        var task = _botClient.GetBusinessConnection(telegramSettings.BusinessConnectionId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}