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

internal class StateContextFactory<T>(
    ILogger<StateContextFactory<T>> logger,
    ITelegramContext botClient,
    IServiceScopeFactory serviceScopeFactory
    ) : IStateContextFactory<T>
    where T : UserBase
{
    private const string ErrorText = "🆘 Произошла ошибка при обработке запроса";

    async Task<IStateContext<T>> IStateContextFactory<T>.CreateStateContextAsync(
        T user,
        StateHistory<T> stateHistory,
        Update update,
        MarkupNextState markupNextState,
        CancellationToken cancellationToken
        )
    {
        var stateContext = new StateContext<T>(logger, botClient);
        await stateContext.CreateStateContextAsync(
            user,
            update,
            markupNextState,
            cancellationToken
            );

        if (stateHistory.CheckAny())
        {
            await RequestAsync(stateContext, stateHistory.StateType, cancellationToken);
        }

        return stateContext;
    }

    private async Task RequestAsync(
        IStateContext<T> stateContext,
        Type stateType,
        CancellationToken cancellationToken
        )
    {
        ArgumentNullException.ThrowIfNull(stateContext);

        var isIStateType = stateType.GetInterfaces().Any(x => x.Name == typeof(IState<T>).Name);

        if (!isIStateType)
        {
            throw new($"Класс {stateType.Name} содержит атрибут {nameof(StateActivatorBaseAttribute)} но не наследуется от {nameof(IState<T>)}");
        }

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var state = scope.ServiceProvider.GetRequiredService(stateType) as IState<T>;

        if (!state.CheckAny())
        {
            throw new("Не смогли активировать состояние");
        }

        await stateContext.SendChatActionAsync(ChatAction.Typing, cancellationToken);

        try
        {
            await state!.HandleAsync(
                stateContext,
                cancellationToken
                );

            await state!.HandleCompleteAsync(
                stateContext,
                cancellationToken
                );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorText);

            await stateContext.SendTextMessageAsync(ErrorText, cancellationToken);

            try
            {
                await state!.HandleErrorAsync(stateContext, ex, cancellationToken);
            }
            catch (Exception ex2)
            {
                logger.LogError(ex2, ErrorText);
            }
        }
    }
}