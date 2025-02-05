using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Abstractions.Factories.Proxies;
using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Common.Contexts.AsyncDisposable.Proxies;

internal class StateContextProxy(
    StateHistory stateHistory,
    IStateProxyFactory stateProxyFactory,
    ITelegramContextProxy telegramContextProxy,
    long chatId
    ) : BaseStateContext(telegramContextProxy, chatId), IStateContext, IStateContextProxyMinimal
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
        => stateProxyFactory.MakeRequestAsync(telegramContextProxy.BotPrefixName, factory => factory.BindStateAsync(ChatId, stateHistory, cancellationToken), cancellationToken);

    public Task UnBindStateAsync(CancellationToken cancellationToken)
        => stateProxyFactory.MakeRequestAsync(telegramContextProxy.BotPrefixName, factory => factory.UnBindStateAsync(ChatId, cancellationToken), cancellationToken);
}