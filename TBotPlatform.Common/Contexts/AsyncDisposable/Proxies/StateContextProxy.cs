using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Abstractions.Factories.Proxies;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Common.Contexts.AsyncDisposable.Proxies;

internal class StateContextProxy(
    StateHistory stateHistory,
    IStateProxyFactory stateBindFactory,
    ITelegramMappingHandler telegramMapping,
    ITelegramContextProxy telegramContextProxy,
    long chatId
    ) : BaseStateContext(telegramMapping, telegramContextProxy, chatId), IStateContext, IStateContextProxyMinimal
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
        => stateBindFactory.MakeRequestAsync(telegramContextProxy.GetBotPrefixName(), factory => factory.BindStateAsync(ChatId, stateHistory, cancellationToken), cancellationToken);

    public Task UnBindStateAsync(CancellationToken cancellationToken)
        => stateBindFactory.MakeRequestAsync(telegramContextProxy.GetBotPrefixName(), factory => factory.UnBindStateAsync(ChatId, cancellationToken), cancellationToken);
}