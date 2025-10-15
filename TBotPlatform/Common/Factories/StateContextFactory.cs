#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Queues;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Factories;

internal class StateContextFactory(ILogger<StateContextFactory> logger, IServiceScopeFactory serviceScopeFactory) : IStateContextFactory
{
    internal const string ErrorText = "🆘 Произошла ошибка при обработке запроса.";

    public IStateContextMinimal GetStateContext<T>(string botName, T user) where T : UserBase
        => GetStateContext(botName, user.ChatId);

    public IStateContextMinimal GetStateContext(string botName, long chatId)
    {
        ArgumentNullException.ThrowIfNull(botName);
        ArgumentNullException.ThrowIfNull(chatId);

        var scope = serviceScopeFactory.CreateAsyncScope();
        var telegramContext = scope.ServiceProvider.GetRequiredKeyedService<ITelegramContext>(botName);
        var stateBindFactory = scope.ServiceProvider.GetRequiredService<IStateBindFactory>();
        var delayQueue = scope.ServiceProvider.GetRequiredService<IDelayQueue>();

        return new StateContext(scope, stateHistory: null, stateBindFactory, telegramContext, delayQueue, chatId);
    }

    public Task<IStateContextMinimal> CreateStateContext<T>(string botName, T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase
        => CreateStateContext(botName, user, stateHistory, update, markupNextState: null, cancellationToken);

    public async Task<IStateContextMinimal> CreateStateContext<T>(
        string botName,
        T user,
        StateHistory stateHistory,
        Update chatUpdate,
        MarkupNextState? markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(botName);
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(chatUpdate);

        var scope = serviceScopeFactory.CreateAsyncScope();
        var telegramContext = scope.ServiceProvider.GetRequiredKeyedService<ITelegramContext>(botName);
        var stateBindFactory = scope.ServiceProvider.GetRequiredService<IStateBindFactory>();
        var delayQueue = scope.ServiceProvider.GetRequiredService<IDelayQueue>();

        var stateContext = new StateContext(scope, stateHistory, stateBindFactory, telegramContext, delayQueue, user.ChatId);
        stateContext.CreateStateContext(chatUpdate, markupNextState);

        await Request(stateContext, user, stateHistory, cancellationToken);

        return stateContext;
    }

    private async Task Request<T>(StateContext stateContext, T user, StateHistory stateHistory, CancellationToken cancellationToken)
        where T : UserBase
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
            throw new($"Не смог активировать состояние {stateType.Name}");
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