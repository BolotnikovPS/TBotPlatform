using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    public async Task<ChatResult> ForwardMessageAsync(long fromChatId, int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        MessageIdValidOrThrow(messageId);

        var result = await telegramContext.ForwardMessageAsync(ChatId, fromChatId, messageId, disableNotification, cancellationToken);

        return telegramMapping.MessageToResult(result);
    }

    public Task<ChatResult> ForwardMessageAsync(long fromChatId, int messageId, CancellationToken cancellationToken)
        => ForwardMessageAsync(fromChatId, messageId, disableNotification: false, cancellationToken);

    public async Task<int> CopyMessageAsync(
        long fromChatId,
        int messageId,
        string caption,
        int replyToMessageId,
        bool allowSendingWithoutReply,
        IReplyMarkup replyMarkup,
        bool disableNotification,
        CancellationToken cancellationToken
        )
    {
        ChatIdValidOrThrow();
        ChatIdValidOrThrow(fromChatId);
        MessageIdValidOrThrow(messageId);

        var result = await telegramContext.CopyMessageAsync(
            ChatId,
            fromChatId,
            messageId,
            caption,
            replyToMessageId,
            allowSendingWithoutReply,
            replyMarkup,
            disableNotification,
            cancellationToken
            );

        return result.Id;
    }
}