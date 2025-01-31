using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using PMode = Telegram.Bot.Types.Enums.ParseMode;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext : ITelegramContext, IAsyncDisposable
{
    private const PMode ParseMode = PMode.Html;

    private readonly TelegramBotClient _botClient;
    private readonly ITelegramContextLog _telegramContextLog;
    private readonly TelegramSettings _telegramSettings;
    private readonly Guid _operationGuid = Guid.NewGuid();

    public TelegramContext(HttpClient client, TelegramSettings telegramSettings, ITelegramContextLog telegramContextLog)
    {
        ArgumentException.ThrowIfNullOrEmpty(telegramSettings.Token);

        client.DefaultRequestHeaders.TryAddWithoutValidation(DefaultHeadersConstant.ContextOperation, _operationGuid.ToString());

        _botClient = new(telegramSettings.Token, client);
        _telegramSettings = telegramSettings;
        _telegramContextLog = telegramContextLog;
    }

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

        var task = _botClient.DeleteMessage(chatId, messageId, cancellationToken);

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

        var task = _botClient.SendChatAction(chatId, chatAction, cancellationToken: cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _telegramContextLog.HandleEnqueueLogAsync(_iteration, _timer.Elapsed.Milliseconds, _operationGuid, CancellationToken.None);
        }
        catch
        {
            // ignored
        }
    }
}