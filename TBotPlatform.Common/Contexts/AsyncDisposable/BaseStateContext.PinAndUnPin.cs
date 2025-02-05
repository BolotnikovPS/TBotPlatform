using Telegram.Bot;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    public Task PinChatMessageAsync(int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        MessageIdValidOrThrow(messageId);

        return telegramContext.PinChatMessage(ChatId, messageId, disableNotification, cancellationToken: cancellationToken);
    }

    public Task PinChatMessageAsync(int messageId, CancellationToken cancellationToken)
        => PinChatMessageAsync(messageId, disableNotification: false, cancellationToken);

    public Task UnpinChatMessageAsync(int messageId, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        MessageIdValidOrThrow(messageId);

        return telegramContext.UnpinChatMessage(ChatId, messageId, cancellationToken: cancellationToken);
    }

    public Task UnpinAllChatMessages(CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        return telegramContext.UnpinAllChatMessages(ChatId, cancellationToken);
    }
}