#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Abstractions.Factories.Proxies;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Contracts.State;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Factories.Proxies;

internal class StateContextProxyFactory(ILogger<StateContextProxyFactory> logger, IStateProxyFactory stateProxyFactory, IServiceScopeFactory serviceScopeFactory)
    : BaseStateContextFactory, IStateContextProxyFactory
{
    public IStateContextProxyMinimal GetStateContext<T>(ITelegramContextProxy telegramContextProxy, T user) where T : UserBase
        => GetStateContext(telegramContextProxy, user.ChatId);

    public IStateContextProxyMinimal GetStateContext(ITelegramContextProxy telegramContextProxy, long chatId)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        return new StateContextProxy(stateHistory: null, stateProxyFactory, GetTelegramContextProxyOrThrow(telegramContextProxy), chatId);
    }

    public Task<IStateContextProxyMinimal> CreateStateContext<T>(
        ITelegramContextProxy telegramContextProxy,
        T user,
        StateHistory stateHistory,
        Update update,
        CancellationToken cancellationToken
        )
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(update);

        return CreateStateContext(telegramContextProxy, user, stateHistory, update, markupNextState: null, cancellationToken);
    }

    public async Task<IStateContextProxyMinimal> CreateStateContext<T>(
        ITelegramContextProxy telegramContextProxy,
        T user,
        StateHistory stateHistory,
        Update chatUpdate,
        MarkupNextState? markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(chatUpdate);

        var stateContext = new StateContextProxy(stateHistory, stateProxyFactory, GetTelegramContextProxyOrThrow(telegramContextProxy), user.ChatId);
        stateContext.CreateStateContext(chatUpdate, markupNextState);

        await Request(stateContext, user, stateHistory, cancellationToken);

        return stateContext;
    }

    public StateResult? GetStateResult(IStateContextProxyMinimal stateContext)
        => stateContext is StateContextProxy context
            ? context.StateResult
            : default;

    internal override async Task Request<T>(IStateContext stateContext, T user, StateHistory stateHistory, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stateContext);
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(stateHistory.StateType);

        var stateType = stateHistory.StateType;

        var isStateType = stateType.GetInterfaces().Any(x => x.Name == typeof(IState<T>).Name);

        if (!isStateType)
        {
            throw new($"Класс {stateType.Name} содержит атрибут {nameof(StateActivatorBaseAttribute)} но не наследуется от {nameof(IState<T>)}");
        }

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var state = scope.ServiceProvider.GetRequiredService(stateType) as IState<T>;

        if (state.IsNull())
        {
            throw new("Не смог активировать состояние");
        }

        try
        {
            await stateContext.SendChatAction(ChatAction.Typing, cancellationToken);
        }
        catch
        {
            // ignored
        }

        Exception? exception = null;
        try
        {
            await state!.Handle(stateContext, user, cancellationToken);

            await state.HandleComplete(stateContext, user, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{stateName}. {errorText}", stateType.Name, ErrorText);
            exception = ex;
        }

        if (exception.IsNull())
        {
            return;
        }

        await state!.HandleError(stateContext, user, exception, cancellationToken);
    }

    private static ITelegramContextProxy GetTelegramContextProxyOrThrow(ITelegramContextProxy telegramContextProxy) => telegramContextProxy ?? throw new TelegramContextProxyException();
}