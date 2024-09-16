#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Contracts.State;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Factories;

internal class StateContextFactory(
    ILogger<StateContextFactory> logger,
    ITelegramContext botClient,
    IStateBind stateBind,
    IServiceScopeFactory serviceScopeFactory,
    ITelegramUpdateHandler telegramUpdateHandler,
    ITelegramMappingHandler telegramMappingHandler
    ) : IStateContextFactory
{
    private const string ErrorText = "🆘 Произошла ошибка при обработке запроса";

    public IStateContextMinimal CreateStateContext<T>(T user) where T : UserBase
        => CreateStateContext(user.ChatId);

    public IStateContextMinimal CreateStateContext(long chatId)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        return new StateContext(stateHistory: null, stateBind, telegramMappingHandler, botClient, chatId);
    }

    public Task<IStateContextMinimal> CreateStateContextAsync<T>(T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(update);

        return CreateStateContextAsync(user, stateHistory, update, markupNextState: null, cancellationToken);
    }

    public Task<IStateContextMinimal> CreateStateContextAsync<T>(T user, StateHistory stateHistory, ChatUpdate chatUpdate, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(chatUpdate);

        return CreateStateContextAsync(user, stateHistory, chatUpdate, markupNextState: null, cancellationToken);
    }

    public async Task<IStateContextMinimal> CreateStateContextAsync<T>(
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

        var chatMessage = await telegramUpdateHandler.GetChatMessageAsync(user.ChatId, update, cancellationToken);

        ArgumentNullException.ThrowIfNull(chatMessage);

        return await CreateStateContextAsync(user, stateHistory, chatMessage, markupNextState, cancellationToken);
    }

    public async Task<IStateContextMinimal> CreateStateContextAsync<T>(
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

        var stateContext = new StateContext(stateHistory, stateBind, telegramMappingHandler, botClient, user.ChatId);
        stateContext.CreateStateContext(chatUpdate, markupNextState);

        await RequestAsync(stateContext, user, stateHistory, cancellationToken);

        return stateContext;
    }

    public StateResult? GetStateResult(IStateContextMinimal stateContext)
        => stateContext is StateContext context
            ? context.StateResult
            : default;

    private async Task RequestAsync<T>(IStateContext stateContext, T user, StateHistory stateHistory, CancellationToken cancellationToken)
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
            logger.LogError(ex, ErrorText);
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

        try
        {
            await state!.HandleErrorAsync(stateContext, user, exception, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorText);
        }
    }
}