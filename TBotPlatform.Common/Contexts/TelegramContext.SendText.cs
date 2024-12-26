#nullable enable
using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    public Task<Message> SendTextMessageAsync(long chatId, string text, IReplyMarkup? replyMarkup, bool disableNotification, CancellationToken cancellationToken)
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            Message = text,
            ReplyMarkup = replyMarkup,
            DisableNotification = disableNotification,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(SendTextMessageAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };

        var task = _botClient.SendMessage(
            chatId,
            text,
            parseMode: ParseMode,
            disableNotification: disableNotification,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            businessConnectionId: telegramSettings?.BusinessConnectionId,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> SendTextMessageAsync(long chatId, string text, bool disableNotification, CancellationToken cancellationToken)
        => SendTextMessageAsync(chatId, text, replyMarkup: null, disableNotification, cancellationToken);
}