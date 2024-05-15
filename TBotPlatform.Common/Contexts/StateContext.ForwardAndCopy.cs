using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext
{
    public Task<Message> ForwardMessageAsync(long fromChatId, int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (messageId.IsDefault())
        {
            throw new MessageIdArgException();
        }

        return botClient.ForwardMessageAsync(ChatId, fromChatId, messageId, disableNotification, cancellationToken);
    }

    public Task<MessageId> CopyMessageAsync(
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
        if (ChatId.IsDefault())
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

        return botClient.CopyMessageAsync(
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
    }
}