using Microsoft.Extensions.Options;
using TBotPlatform.Common.Contracts;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Factories;

internal partial class StateFactory(ICacheService cache, StateFactoryDataCollection stateFactoryDataCollection, IOptions<StateFactorySettings> stateFactorySettings) : IStateFactory
{
    public StateHistory GetStateByNameOrDefault(string nameOfState = "")
    {
        if (nameOfState.IsNull())
        {
            return CreateContextFunc();
        }

        var newState = stateFactoryDataCollection.FirstOrDefault(q => q.StateTypeName == nameOfState);

        return ConvertStateFactoryData(newState);
    }

    public async Task<StateHistory> GetStateByButtonsTypeOrDefault(long chatId, string buttonTypeValue, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(buttonTypeValue);

        var newState = stateFactoryDataCollection.FirstOrDefault(
            q => q.ButtonsTypes.CheckAny()
                 && q.ButtonsTypes.Any(z => z == buttonTypeValue)
            );

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmpty(chatId, cancellationToken);

        var checkNewState = newState.IsNotNull()
                            && !newState!.IsInlineState
                            && !newState.StateTypeName.In(statesInMemoryOrEmpty?.LastOrDefault());

        if (!checkNewState)
        {
            return ConvertStateFactoryData(newState);
        }

        statesInMemoryOrEmpty?.Add(newState.StateTypeName);
        await UpdateValueState(statesInMemoryOrEmpty, chatId, cancellationToken);

        await UnBindState(chatId, cancellationToken);

        return ConvertStateFactoryData(newState);
    }

    public async Task<StateHistory> GetStateByCommandsTypeOrDefault(long chatId, string commandTypeValue, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(commandTypeValue);

        var newState = stateFactoryDataCollection
           .FirstOrDefault(
                q => q.CommandsTypes.CheckAny()
                     && q.CommandsTypes.Any(z => z == commandTypeValue)
                );

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmpty(chatId, cancellationToken);

        var checkNewState = newState.IsNotNull()
                            && !newState!.IsInlineState
                            && !newState.StateTypeName.In(statesInMemoryOrEmpty?.LastOrDefault());

        if (!checkNewState)
        {
            return ConvertStateFactoryData(newState);
        }

        statesInMemoryOrEmpty?.Add(newState.StateTypeName);
        await UpdateValueState(statesInMemoryOrEmpty, chatId, cancellationToken);

        await UnBindState(chatId, cancellationToken);

        return ConvertStateFactoryData(newState);
    }

    public StateHistory GetStateByTextsTypeOrDefault(long chatId, string textTypeValue)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(textTypeValue);

        var newState = stateFactoryDataCollection
           .FirstOrDefault(
                q => q.TextsTypes.CheckAny()
                     && q.TextsTypes.Any(z => z == textTypeValue)
                );

        return ConvertStateFactoryData(newState);
    }

    public async Task<StateHistory> GetStateMain(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmpty(chatId, cancellationToken);
        statesInMemoryOrEmpty.Clear();
        await UpdateValueState(statesInMemoryOrEmpty, chatId, cancellationToken);

        await UnBindState(chatId, cancellationToken);

        return CreateContextFunc();
    }

    public async Task<StateHistory> GetStatePreviousOrMain(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmpty(chatId, cancellationToken);
        if (statesInMemoryOrEmpty.Count == 0)
        {
            return CreateContextFunc();
        }

        statesInMemoryOrEmpty.RemoveAt(statesInMemoryOrEmpty.Count - 1);

        return await GetLastStateWithMenuOrMain(statesInMemoryOrEmpty, chatId, cancellationToken);
    }

    public async Task<StateHistory> GetLastStateWithMenu(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmpty(chatId, cancellationToken);
        var result = await GetLastStateWithMenuOrMain(statesInMemoryOrEmpty, chatId, cancellationToken);

        return result.IsNotNull() ? result : null;
    }

    public StateHistory LockState => GetStateHistory(z => z.IsLockUserState);

    public StateHistory RegistrationState => GetStateHistory(z => z.IsRegistrationState);

    public async Task<StateHistory> GetBindStateOrNull(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var value = await GetBindStateName(chatId, cancellationToken);

        var state = stateFactoryDataCollection.FirstOrDefault(z => z.StateTypeName == value);

        return value.IsNotNull() ? Convert(state) : null;
    }

    public Task BindState(long chatId, StateHistory state, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(state);

        var value = stateFactoryDataCollection.FirstOrDefault(z => z.StateTypeName == state.StateType.Name);

        if (value.IsNull())
        {
            throw new("Состояние не найдено");
        }

        return AddBindState(chatId, value?.StateTypeName, cancellationToken);
    }

    public Task UnBindState(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        return RemoveBindState(chatId, cancellationToken);
    }

    public async Task<bool> HasBindState(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var value = await GetBindStateName(chatId, cancellationToken);

        return value.CheckAny();
    }

    private async Task<StateHistory> GetLastStateWithMenuOrMain(List<string> statesInMemoryOrEmpty, long chatId, CancellationToken cancellationToken)
    {
        var lastStateToRemove = statesInMemoryOrEmpty.LastOrDefault();

        if (lastStateToRemove.IsNull())
        {
            await UpdateValueState(statesInMemoryOrEmpty, chatId, cancellationToken);

            await UnBindState(chatId, cancellationToken);

            return CreateContextFunc();
        }

        while (true)
        {
            var lastState = statesInMemoryOrEmpty.LastOrDefault();

            if (lastState.IsNull())
            {
                await UpdateValueState(statesInMemoryOrEmpty, chatId, cancellationToken);

                await UnBindState(chatId, cancellationToken);

                return CreateContextFunc();
            }

            var state = stateFactoryDataCollection.FirstOrDefault(x => x.StateTypeName == lastState);

            if (state?.MenuTypeName != null)
            {
                await UpdateValueState(statesInMemoryOrEmpty, chatId, cancellationToken);

                await UnBindState(chatId, cancellationToken);

                return Convert(state);
            }

            statesInMemoryOrEmpty.Remove(lastState);
        }
    }

    private StateHistory GetStateHistory(Func<StateFactoryData, bool> request)
    {
        var state = stateFactoryDataCollection.FirstOrDefault(request);

        return state.IsNotNull() ? Convert(state) : null;
    }
}