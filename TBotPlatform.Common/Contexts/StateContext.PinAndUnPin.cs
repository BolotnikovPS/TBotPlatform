using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext
{
    public Task PinChatMessageAsync(int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (messageId.IsDefault())
        {
            throw new MessageIdArgException();
        }

        return botClient.PinChatMessageAsync(ChatId, messageId, disableNotification, cancellationToken);
    }

    public Task UnpinChatMessageAsync(int messageId, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (messageId.IsDefault())
        {
            throw new MessageIdArgException();
        }

        return botClient.UnpinChatMessageAsync(ChatId, messageId, cancellationToken);
    }

    public Task UnpinAllChatMessages(CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        return botClient.UnpinAllChatMessages(ChatId, cancellationToken);
    }
}