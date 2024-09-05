using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.Users;
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

        return new StateContext(null, stateBind, telegramMappingHandler, botClient, chatId);
    }

    public Task<IStateContext> CreateStateContextAsync<T>(T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(update);

        return CreateStateContextAsync(user, stateHistory, update, null, cancellationToken);
    }

    public Task<IStateContext> CreateStateContextAsync<T>(T user, StateHistory stateHistory, ChatUpdate chatUpdate, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(chatUpdate);

        return CreateStateContextAsync(user, stateHistory, chatUpdate, null, cancellationToken);
    }

    public async Task<IStateContext> CreateStateContextAsync<T>(
        T user,
        StateHistory stateHistory,
        Update update,
        MarkupNextState markupNextState,
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

    public async Task<IStateContext> CreateStateContextAsync<T>(
        T user,
        StateHistory stateHistory,
        ChatUpdate chatUpdate,
        MarkupNextState markupNextState,
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

    public async Task UpdateMarkupByStateAsync<T>(T user, IStateContextMinimal stateContext, StateHistory stateHistory, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateContext);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(stateHistory.MenuStateType);

        var menuStateType = stateHistory.MenuStateType;
        var isMenuType = menuStateType.GetInterfaces().Any(x => x.Name == nameof(IMenuButton));

        if (!isMenuType)
        {
            throw new($"Класс {menuStateType.Name} не наследуется от {nameof(IMenuButton)}");
        }

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var menuButtons = scope.ServiceProvider.GetRequiredService(menuStateType) as IMenuButton;

        if (menuButtons.IsNull())
        {
            throw new("Не смогли активировать состояние");
        }

        var menu = await menuButtons!.GetMarkUpAsync(user);
        await stateContext.UpdateMarkupAsync(menu, cancellationToken);
    }

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
            throw new("Не смогли активировать состояние");
        }

        try
        {
            await stateContext.SendChatActionAsync(ChatAction.Typing, cancellationToken);
        }
        catch
        {
            // ignored
        }

        Exception exception = null;
        try
        {
            await state!.HandleAsync(stateContext, user, cancellationToken);

            await state!.HandleCompleteAsync(stateContext, user, cancellationToken);
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