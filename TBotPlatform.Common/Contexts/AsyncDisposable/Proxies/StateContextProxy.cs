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
    public async Task<T> MakeRequestToOtherChat<T>(long newChatId, Func<IStateContextMinimal, Task<T>> request)
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

    public Task BindState(CancellationToken cancellationToken)
        => stateProxyFactory.MakeRequest(telegramContextProxy.BotPrefixName, factory => factory.BindState(ChatId, stateHistory, cancellationToken), cancellationToken);

    public Task UnBindState(CancellationToken cancellationToken)
        => stateProxyFactory.MakeRequest(telegramContextProxy.BotPrefixName, factory => factory.UnBindState(ChatId, cancellationToken), cancellationToken);
}