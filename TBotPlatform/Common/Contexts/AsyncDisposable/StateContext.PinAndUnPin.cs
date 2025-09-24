using Telegram.Bot;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public Task PinChatMessage(int messageId, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        MessageIdValidOrThrow(messageId);

        return telegramContext.PinChatMessage(chatId, messageId, disableNotification, cancellationToken: cancellationToken);
    }

    public Task PinChatMessage(int messageId, CancellationToken cancellationToken)
        => PinChatMessage(messageId, disableNotification: false, cancellationToken);

    public Task UnpinChatMessage(int messageId, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        MessageIdValidOrThrow(messageId);

        return telegramContext.UnpinChatMessage(chatId, messageId, cancellationToken: cancellationToken);
    }

    public Task UnpinAllChatMessages(CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        return telegramContext.UnpinAllChatMessages(chatId, cancellationToken);
    }
}