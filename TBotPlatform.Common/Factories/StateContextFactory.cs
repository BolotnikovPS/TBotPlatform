#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Contracts.State;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Factories;

internal class StateContextFactory(
    ILogger<StateContextFactory> logger,
    ITelegramContext telegramContext,
    IStateBindFactory stateBindFactory,
    IServiceScopeFactory serviceScopeFactory
    ) : BaseStateContextFactory, IStateContextFactory
{
    public IStateContextMinimal GetStateContext<T>(T user) where T : UserBase
        => GetStateContext(user.ChatId);

    public IStateContextMinimal GetStateContext(long chatId)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        return new StateContext(stateHistory: null, stateBindFactory, telegramContext, chatId);
    }

    public Task<IStateContextMinimal> CreateStateContext<T>(T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(update);

        return CreateStateContext(user, stateHistory, update, markupNextState: null, cancellationToken);
    }

    public async Task<IStateContextMinimal> CreateStateContext<T>(
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

        var stateContext = new StateContext(stateHistory, stateBindFactory, telegramContext, user.ChatId);
        stateContext.CreateStateContext(chatUpdate, markupNextState);

        await Request(stateContext, user, stateHistory, cancellationToken);

        return stateContext;
    }

    public StateResult? GetStateResult(IStateContextMinimal stateContext)
        => stateContext is BaseStateContext context
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

        try
        {
            await stateContext.SendTextMessage(ErrorText, cancellationToken);
        }
        catch
        {
            // ignored
        }

        await state!.HandleError(stateContext, user, exception, cancellationToken);
    }
}