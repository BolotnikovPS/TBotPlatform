using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public async Task<ChatResult> ForwardMessageAsync(long fromChatId, int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        if (chatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (messageId.IsDefault())
        {
            throw new MessageIdArgException();
        }

        var result = await botClient.ForwardMessageAsync(chatId, fromChatId, messageId, disableNotification, cancellationToken);

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
        if (chatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (fromChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (messageId.IsDefault())
        {
            throw new MessageIdArgException();
        }

        var result = await botClient.CopyMessageAsync(
            chatId,
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