#nullable enable
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Extension;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Handlers.State;

public abstract class StartReceivingHandlerBase<TUser>(IStateFactory stateFactory, IStateContextFactory stateContextFactory) : IStartReceivingHandler
    where TUser : UserBase
{
    public async Task<IResult> HandleUpdate(
        string botName,
        Update update,
        MarkupNextState? markupNextState,
        TelegramMessageUserData telegramData,
        CancellationToken cancellationToken
        )
    {
        var chatId = telegramData.ChatOrNull?.Id
                     ?? throw new InvalidOperationException("В обновлении отсутствует чат.");

        var user = await GetOrCreateUser(botName, telegramData, cancellationToken);

        await BeforeHandleAsync(botName, update, user, cancellationToken);

        var state = await ResolveState(botName, chatId, update, markupNextState, user, cancellationToken);
        if (!state.IsSuccess)
        {
            return Result.Failure(state.Error ?? ErrorResult.NotFound("Состояние не найдено"));
        }

        await using var context = await stateContextFactory.CreateStateContext(botName, user, state.Value, update, markupNextState, cancellationToken);

        await AfterHandleAsync(botName, update, user, cancellationToken);

        return Result.Success();
    }

    protected abstract Task<TUser> GetOrCreateUser(string botName, TelegramMessageUserData telegramData, CancellationToken cancellationToken);

    /// <summary>
    /// Вызывается перед обработкой обновления.
    /// </summary>
    protected virtual Task BeforeHandleAsync(string botName, Update update, TUser user, CancellationToken cancellationToken)
        => Task.CompletedTask;

    /// <summary>
    /// Вызывается после успешной обработки обновления.
    /// </summary>
    protected virtual Task AfterHandleAsync(string botName, Update update, TUser user, CancellationToken cancellationToken)
        => Task.CompletedTask;

    protected virtual async Task<IResult<StateHistory>> ResolveState(
        string botName,
        long chatId,
        Update update,
        MarkupNextState? markupNextState,
        TUser user,
        CancellationToken cancellationToken
        )
    {
        if (markupNextState.IsNotNull() && markupNextState!.State.CheckAny())
        {
            return stateFactory.GetStateByNameOrDefault(botName, markupNextState.State);
        }

        var bind = await stateFactory.HasBindState(botName, chatId, cancellationToken);
        if (bind.IsSuccess && bind.Value)
        {
            return await stateFactory.GetBindStateOrNull(botName, chatId, cancellationToken);
        }

        if (update.Type == UpdateType.Message && update.Message.TryGetText(out var text) && text.CheckAny())
        {
            if (text![0] == '/')
            {
                var command = text.Split([' ', '\n'], 2)[0].Split('@')[0];
                return await stateFactory.GetStateByCommandsTypeOrDefault(botName, chatId, command, cancellationToken);
            }

            return await stateFactory.GetStateByButtonsTypeOrDefault(botName, chatId, text, cancellationToken);
        }

        return await stateFactory.GetLastStateWithMenu(botName, chatId, cancellationToken);
    }
}
