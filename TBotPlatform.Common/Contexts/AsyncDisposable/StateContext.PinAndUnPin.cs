using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public Task PinChatMessageAsync(int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        if (chatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (messageId.IsDefault())
        {
            throw new MessageIdArgException();
        }

        return botClient.PinChatMessageAsync(chatId, messageId, disableNotification, cancellationToken);
    }

    public Task PinChatMessageAsync(int messageId, CancellationToken cancellationToken)
        => PinChatMessageAsync(messageId, false, cancellationToken);

    public Task UnpinChatMessageAsync(int messageId, CancellationToken cancellationToken)
    {
        if (chatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (messageId.IsDefault())
        {
            throw new MessageIdArgException();
        }

        return botClient.UnpinChatMessageAsync(chatId, messageId, cancellationToken);
    }

    public Task UnpinAllChatMessages(CancellationToken cancellationToken)
    {
        if (chatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        return botClient.UnpinAllChatMessages(chatId, cancellationToken);
    }
}