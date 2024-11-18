namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    public Task PinChatMessageAsync(int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        MessageIdValidOrThrow(messageId);

        return telegramContext.PinChatMessageAsync(ChatId, messageId, disableNotification, cancellationToken);
    }

    public Task PinChatMessageAsync(int messageId, CancellationToken cancellationToken)
        => PinChatMessageAsync(messageId, disableNotification: false, cancellationToken);

    public Task UnpinChatMessageAsync(int messageId, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        MessageIdValidOrThrow(messageId);

        return telegramContext.UnpinChatMessageAsync(ChatId, messageId, cancellationToken);
    }

    public Task UnpinAllChatMessages(CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        return telegramContext.UnpinAllChatMessagesAsync(ChatId, cancellationToken);
    }
}