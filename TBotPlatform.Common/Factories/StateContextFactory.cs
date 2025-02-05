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
    public IStateContextMinimal CreateStateContext<T>(T user) where T : UserBase
        => CreateStateContext(user.ChatId);

    public IStateContextMinimal CreateStateContext(long chatId)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        return new StateContext(stateHistory: null, stateBindFactory, telegramContext, chatId);
    }

    public Task<IStateContextMinimal> CreateStateContextAsync<T>(T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(update);

        return CreateStateContextAsync(user, stateHistory, update, markupNextState: null, cancellationToken);
    }

    public async Task<IStateContextMinimal> CreateStateContextAsync<T>(
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

        await RequestAsync(stateContext, user, stateHistory, cancellationToken);

        return stateContext;
    }

    public StateResult? GetStateResult(IStateContextMinimal stateContext)
        => stateContext is BaseStateContext context
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

        try
        {
            await stateContext.SendTextMessageAsync(ErrorText, cancellationToken);
        }
        catch
        {
            // ignored
        }

        await state!.HandleErrorAsync(stateContext, user, exception, cancellationToken);
    }
}