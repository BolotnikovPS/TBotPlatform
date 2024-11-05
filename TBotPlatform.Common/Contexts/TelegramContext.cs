using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using PMode = Telegram.Bot.Types.Enums.ParseMode;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext(HttpClient client, TelegramSettings telegramSettings, ITelegramContextLog telegramContextLog) : ITelegramContextProxy
{
    private const PMode ParseMode = PMode.Html;

    private readonly TelegramBotClient _botClient = new(GetTelegramToken(telegramSettings), client);
    private readonly Guid _operationGuid = Guid.NewGuid();

    public Guid GetCurrentOperation() => _operationGuid;

    public Task<T> MakeRequestAsync<T>(Func<ITelegramBotClient, Task<T>> request, TelegramContextLogMessage logMessage, CancellationToken cancellationToken)
        => ExecuteEnqueueSafety(request.Invoke(_botClient), logMessage, cancellationToken);

    public Task<T> MakeRequestAsync<T>(Func<ITelegramBotClient, Task<T>> request, CancellationToken cancellationToken)
        => Enqueue(() => request.Invoke(_botClient), cancellationToken);

    public Task MakeRequestAsync(Func<ITelegramBotClient, Task> request, TelegramContextLogMessage logMessage, CancellationToken cancellationToken)
        => ExecuteEnqueueSafety(request.Invoke(_botClient), logMessage, cancellationToken);

    public Task MakeRequestAsync(Func<ITelegramBotClient, Task> request, CancellationToken cancellationToken)
        => Enqueue(() => request.Invoke(_botClient), cancellationToken);

    public Task DeleteMessageAsync(long chatId, int messageId, CancellationToken cancellationToken)
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            MessageId = messageId,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(DeleteMessageAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };

        var task = _botClient.DeleteMessageAsync(chatId, messageId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task SendChatActionAsync(long chatId, ChatAction chatAction, CancellationToken cancellationToken)
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            ChatAction = chatAction,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(SendChatActionAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };

        var task = _botClient.SendChatActionAsync(chatId, chatAction, cancellationToken: cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    private static string GetTelegramToken(TelegramSettings telegramSettings)
    {
        ArgumentException.ThrowIfNullOrEmpty(telegramSettings.Token);

        return telegramSettings.Token;
    }

    public ValueTask DisposeAsync()
    {
        telegramSettings = null;

        return ValueTask.CompletedTask;
    }
}