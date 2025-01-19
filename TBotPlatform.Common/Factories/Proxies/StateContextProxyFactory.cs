#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Abstractions.Factories.Proxies;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;
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
    private ITelegramContextProxy? TelegramContextProxy { get; set; }

    public void SetBot(ITelegramContextProxy telegramContextProxy)
    {
        TelegramContextProxy = telegramContextProxy;
    }

    public IStateContextProxyMinimal CreateStateContext<T>(T user) where T : UserBase
        => CreateStateContext(user.ChatId);

    public IStateContextProxyMinimal CreateStateContext(long chatId)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        return new StateContextProxy(stateHistory: null, stateProxyFactory, GetTelegramMappingHandler, GetTelegramContextProxyOrThrow, chatId);
    }

    public Task<IStateContextProxyMinimal> CreateStateContextAsync<T>(T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(update);

        return CreateStateContextAsync(user, stateHistory, update, markupNextState: null, cancellationToken);
    }

    public Task<IStateContextProxyMinimal> CreateStateContextAsync<T>(T user, StateHistory stateHistory, ChatUpdate chatUpdate, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(chatUpdate);

        return CreateStateContextAsync(user, stateHistory, chatUpdate, markupNextState: null, cancellationToken);
    }

    public async Task<IStateContextProxyMinimal> CreateStateContextAsync<T>(
        T user,
        StateHistory stateHistory,
        Update update,
        MarkupNextState? markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(update);

        var chatMessage = await new TelegramChatHandler(GetTelegramContextProxyOrThrow).GetChatMessageAsync(user.ChatId, update, cancellationToken);

        ArgumentNullException.ThrowIfNull(chatMessage);

        return await CreateStateContextAsync(user, stateHistory, chatMessage, markupNextState, cancellationToken);
    }

    public async Task<IStateContextProxyMinimal> CreateStateContextAsync<T>(
        T user,
        StateHistory stateHistory,
        ChatUpdate chatUpdate,
        MarkupNextState? markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(chatUpdate);

        var stateContext = new StateContextProxy(stateHistory, stateProxyFactory, GetTelegramMappingHandler, GetTelegramContextProxyOrThrow, user.ChatId);
        stateContext.CreateStateContext(chatUpdate, markupNextState);

        await RequestAsync(stateContext, user, stateHistory, cancellationToken);

        return stateContext;
    }

    public StateResult? GetStateResult(IStateContextProxyMinimal stateContext)
        => stateContext is StateContextProxy context
            ? context.StateResult
            : default;

    internal override async Task RequestAsync<T>(IStateContext stateContext, T user, StateHistory stateHistory, CancellationToken cancellationToken)
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
            await stateContext.SendChatActionAsync(ChatAction.Typing, cancellationToken);
        }
        catch
        {
            // ignored
        }

        Exception? exception = null;
        try
        {
            await state!.HandleAsync(stateContext, user, cancellationToken);

            await state.HandleCompleteAsync(stateContext, user, cancellationToken);
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

        await state!.HandleErrorAsync(stateContext, user, exception, cancellationToken);
    }

    private ITelegramMappingHandler GetTelegramMappingHandler => new TelegramChatHandler(GetTelegramContextProxyOrThrow);

    private ITelegramContextProxy GetTelegramContextProxyOrThrow => TelegramContextProxy ?? throw new TelegramContextProxyException();
}