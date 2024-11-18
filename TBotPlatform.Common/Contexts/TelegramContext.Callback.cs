using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    public Task AnswerCallbackQueryAsync(
        string callbackQueryId,
        string text,
        bool showAlert,
        string url,
        int? cacheTime,
        CancellationToken cancellationToken
        )
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(AnswerCallbackQueryAsync),
            ChatId = 0,
            MessageBody = new(),
        };

        var task = _botClient.AnswerCallbackQuery(callbackQueryId, text, showAlert, url, cacheTime, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}