using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal class StateContext(
    StateHistory stateHistory,
    IStateBindFactory stateBindFactory,
    ITelegramContext telegramContext,
    long chatId
    ) : BaseStateContext(telegramContext, chatId), IStateContext
{
    public async Task<T> MakeRequestToOtherChatAsync<T>(long newChatId, Func<IStateContextMinimal, Task<T>> request)
    {
        ChatIdValidOrThrow(newChatId);

        if (ChatId == newChatId)
        {
            return await request.Invoke(this);
        }

        var baseChatId = ChatId;
        try
        {
            ChatId = newChatId;
            return await request.Invoke(this);
        }
        finally
        {
            ChatId = baseChatId;
        }
    }

    public Task BindStateAsync(CancellationToken cancellationToken)
        => stateBindFactory.BindStateAsync(ChatId, stateHistory, cancellationToken);

    public Task UnBindStateAsync(CancellationToken cancellationToken)
        => stateBindFactory.UnBindStateAsync(ChatId, cancellationToken);
}