using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Factories;

internal class StateContextFactory(ILogger<StateContextFactory> logger, ITelegramContext botClient, IServiceScopeFactory serviceScopeFactory) : IStateContextFactory
{
    private const string ErrorText = "🆘 Произошла ошибка при обработке запроса";

    async Task<IStateContext> IStateContextFactory.CreateStateContextAsync<T>(
        T user,
        StateHistory<T> stateHistory,
        Update update,
        MarkupNextState markupNextState,
        CancellationToken cancellationToken
        )
    {
        var stateContext = new StateContext(logger, botClient);
        await stateContext.CreateStateContextAsync(
            user,
            update,
            markupNextState,
            cancellationToken
            );

        if (stateHistory.IsNotNull())
        {
            await RequestAsync(stateContext, user, stateHistory.StateType, cancellationToken);
        }

        return stateContext;
    }

    private async Task RequestAsync<T>(IStateContext stateContext, T user, Type stateType, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(stateContext);

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

        await stateContext.SendChatActionAsync(ChatAction.Typing, cancellationToken);

        try
        {
            await state!.HandleAsync(stateContext, user, cancellationToken);

            await state!.HandleCompleteAsync(stateContext, user, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorText);

            await stateContext.SendTextMessageAsync(ErrorText, cancellationToken);

            try
            {
                await state!.HandleErrorAsync(stateContext, user, ex, cancellationToken);
            }
            catch (Exception ex2)
            {
                logger.LogError(ex2, ErrorText);
            }
        }
    }
}