using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using PMode = Telegram.Bot.Types.Enums.ParseMode;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext(ILogger<TelegramContext> logger, HttpClient client, TelegramSettings telegramSettings, ITelegramContextLog telegramContextLog) : ITelegramContext
{
    private const PMode ParseMode = PMode.Html;

    private readonly TelegramBotClient _botClient = new(telegramSettings.Token, client);
    private readonly Guid _operationGuid = Guid.NewGuid();

    public Guid GetCurrentOperation() => _operationGuid;

    public Task<Update[]> GetUpdatesAsync(int offset, UpdateType[] allowedUpdates, CancellationToken cancellationToken)
    {
        var task = _botClient.GetUpdatesAsync(
            offset,
            allowedUpdates: allowedUpdates.IsNotNull() ? allowedUpdates : null,
            cancellationToken: cancellationToken
            );

        return Enqueue(() => task, cancellationToken);
    }

    public Task<User> GetBotInfoAsync(CancellationToken cancellationToken)
    {
        return Enqueue(
            () => _botClient.GetMeAsync(cancellationToken),
            cancellationToken
            );
    }

    public Task DeleteMessageAsync(long chatId, int messageId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(DeleteMessageAsync),
            ChatId = chatId,
            MessageId = messageId,
        };

        var task = _botClient.DeleteMessageAsync(chatId, messageId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task SendChatActionAsync(long chatId, ChatAction chatAction, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(SendChatActionAsync),
            ChatId = chatId,
            ChatAction = chatAction,
        };

        var task = _botClient.SendChatActionAsync(chatId, chatAction, cancellationToken: cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}