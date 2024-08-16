using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Handlers;
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
    IServiceScopeFactory serviceScopeFactory,
    ITelegramUpdateHandler telegramUpdateHandler,
    ITelegramMappingHandler telegramMappingHandler
    ) : IStateContextFactory
{
    private const string ErrorText = "🆘 Произошла ошибка при обработке запроса";
    
    public IStateContext CreateStateContext<T>(T user) where T : UserBase
        => CreateStateContext(user.ChatId);

    public IStateContext CreateStateContext(long chatId)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        return new StateContext(telegramMappingHandler, botClient, chatId);
    }

    public Task<IStateContext> CreateStateContextAsync<T>(T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase
        => CreateStateContextAsync(user, stateHistory, update, null, cancellationToken);

    public Task<IStateContext> CreateStateContextAsync<T>(T user, StateHistory stateHistory, ChatUpdate chatUpdate, CancellationToken cancellationToken)
        where T : UserBase
        => CreateStateContextAsync(user, stateHistory, chatUpdate, null, cancellationToken);

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

        var stateContext = new StateContext(telegramMappingHandler, botClient, user.ChatId);
        stateContext.CreateStateContext(chatUpdate, markupNextState);

        if (stateHistory.IsNotNull())
        {
            await RequestAsync(stateContext, user, stateHistory.StateType, cancellationToken);
        }

        return stateContext;
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

        var chatMessage = await telegramUpdateHandler.GetChatMessageAsync(user.ChatId, update, cancellationToken);

        return await CreateStateContextAsync(user, stateHistory, chatMessage, markupNextState, cancellationToken);
    }

    public async Task UpdateMarkupByStateAsync<T>(T user, IStateContext stateContext, StateHistory stateHistory, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateContext);
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(stateHistory.MenuStateType);

        var isMenuType = stateHistory.MenuStateType.GetInterfaces().Any(x => x.Name == nameof(IMenuButton));

        if (!isMenuType)
        {
            throw new($"Класс {stateHistory.MenuStateType.Name} не наследуется от {nameof(IMenuButton)}");
        }

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var menuButtons = scope.ServiceProvider.GetRequiredService(stateHistory.MenuStateType) as IMenuButton;

        if (menuButtons.IsNull())
        {
            throw new("Не смогли активировать состояние");
        }

        var menu = await menuButtons!.GetMarkUpAsync(user);
        await stateContext.UpdateMarkupAsync(menu, cancellationToken);
    }


    private async Task RequestAsync<T>(IStateContext stateContext, T user, Type stateType, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(stateContext);
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateType);

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