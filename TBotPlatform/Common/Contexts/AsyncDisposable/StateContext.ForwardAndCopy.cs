using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public Task<Message> ForwardMessage(long fromChatId, int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        MessageIdValidOrThrow(messageId);

        return telegramContext.ForwardMessage(ChatId, fromChatId, messageId, messageThreadId: null, disableNotification, cancellationToken: cancellationToken);
    }

    public Task<Message> ForwardMessage(long fromChatId, int messageId, CancellationToken cancellationToken)
        => ForwardMessage(fromChatId, messageId, disableNotification: false, cancellationToken);

    public async Task<int> CopyMessage(
        long fromChatId,
        int messageId,
        string caption,
        int replyToMessageId,
        ReplyMarkup replyMarkup,
        bool disableNotification,
        CancellationToken cancellationToken
        )
    {
        ChatIdValidOrThrow();
        ChatIdValidOrThrow(fromChatId);
        MessageIdValidOrThrow(messageId);

        var result = await telegramContext.CopyMessage(
            ChatId,
            fromChatId,
            messageId,
            caption,
            ParseMode,
            replyToMessageId,
            replyMarkup,
            disableNotification: disableNotification,
            cancellationToken: cancellationToken
            );

        return result.Id;
    }
}