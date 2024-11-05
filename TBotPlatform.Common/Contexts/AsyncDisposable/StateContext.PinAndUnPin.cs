using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

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

        return telegramContext.PinChatMessageAsync(ChatId, messageId, disableNotification, cancellationToken);
    }

    public Task PinChatMessageAsync(int messageId, CancellationToken cancellationToken)
        => PinChatMessageAsync(messageId, disableNotification: false, cancellationToken);

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

        return telegramContext.UnpinChatMessageAsync(ChatId, messageId, cancellationToken);
    }

    public Task UnpinAllChatMessages(CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        return telegramContext.UnpinAllChatMessagesAsync(ChatId, cancellationToken);
    }
}