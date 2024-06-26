using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Factories;

internal partial class StateFactory<T>(
    ICacheService cache,
    StateFactoryDataCollection stateFactoryDataCollection,
    StateFactorySettings stateFactorySettings
    ) : IStateFactory<T>
    where T : UserBase
{
    public StateHistory<T> GetStateByNameOrDefault(string nameOfState = "")
    {
        if (!nameOfState.CheckAny())
        {
            return CreateContextFunc();
        }

        var newState = stateFactoryDataCollection.FirstOrDefault(q => q.StateTypeName == nameOfState);

        return ConvertStateFactoryData(newState);
    }

    public async Task<StateHistory<T>> GetStateByButtonsTypeOrDefaultAsync(long chatId, string buttonTypeValue, CancellationToken cancellationToken)
    {
        var newState = stateFactoryDataCollection
           .FirstOrDefault(
                q => q.ButtonsTypes.CheckAny()
                     && q.ButtonsTypes.Any(z => z == buttonTypeValue)
                );

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmptyAsync(chatId, cancellationToken);

        var checkNewState = newState.CheckAny()
                            && !newState!.IsInlineState
                            && !newState.StateTypeName.In(statesInMemoryOrEmpty?.LastOrDefault());

        if (!checkNewState)
        {
            return ConvertStateFactoryData(newState);
        }

        statesInMemoryOrEmpty?.Add(newState.StateTypeName);
        await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);

        await UnBindStateAsync(chatId, cancellationToken);

        return ConvertStateFactoryData(newState);
    }

    public async Task<StateHistory<T>> GetStateByCommandsTypeOrDefaultAsync(long chatId, string commandTypeValue, CancellationToken cancellationToken)
    {
        var newState = stateFactoryDataCollection
           .FirstOrDefault(
                q => q.CommandsTypes.CheckAny()
                     && q.CommandsTypes.Any(z => z == commandTypeValue)
                );

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmptyAsync(chatId, cancellationToken);

        var checkNewState = newState.CheckAny()
                            && !newState!.IsInlineState
                            && !newState.StateTypeName.In(statesInMemoryOrEmpty?.LastOrDefault());

        if (!checkNewState)
        {
            return ConvertStateFactoryData(newState);
        }

        statesInMemoryOrEmpty?.Add(newState.StateTypeName);
        await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);

        await UnBindStateAsync(chatId, cancellationToken);

        return ConvertStateFactoryData(newState);
    }

    public StateHistory<T> GetStateByTextsTypeOrDefault(long chatId, string textTypeValue)
    {
        var newState = stateFactoryDataCollection
           .FirstOrDefault(
                q => q.TextsTypes.CheckAny()
                     && q.TextsTypes.Any(z => z == textTypeValue)
                );

        return ConvertStateFactoryData(newState);
    }

    public async Task<StateHistory<T>> GetStateMainAsync(long chatId, CancellationToken cancellationToken)
    {
        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmptyAsync(chatId, cancellationToken);
        statesInMemoryOrEmpty.Clear();
        await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);

        await UnBindStateAsync(chatId, cancellationToken);

        return CreateContextFunc();
    }

    public async Task<StateHistory<T>> GetStatePreviousOrMainAsync(long chatId, CancellationToken cancellationToken)
    {
        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmptyAsync(chatId, cancellationToken);
        if (statesInMemoryOrEmpty.Count == 0)
        {
            return CreateContextFunc();
        }

        statesInMemoryOrEmpty.RemoveAt(statesInMemoryOrEmpty.Count - 1);

        return await GetLastStateWithMenuOrMainAsync(statesInMemoryOrEmpty, chatId, cancellationToken);
    }

    public async Task<IMenuButton<T>> GetLastMenuAsync(long chatId, CancellationToken cancellationToken)
    {
        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmptyAsync(chatId, cancellationToken);
        var result = await GetLastStateWithMenuOrMainAsync(statesInMemoryOrEmpty, chatId, cancellationToken);

        return result.CheckAny() ? result.MenuState : default;
    }

    public StateHistory<T> GetLockState()
    {
        var state = stateFactoryDataCollection.FirstOrDefault(z => z.IsLockUserState);

        return Convert(state);
    }

    public async Task<StateHistory<T>> GetBindStateOrNullAsync(long chatId, CancellationToken cancellationToken)
    {
        var value = await GetBindStateNameAsync(chatId, cancellationToken);

        var state = stateFactoryDataCollection.FirstOrDefault(z => z.StateTypeName == value);

        return value.CheckAny()
            ? Convert(state)
            : null;
    }

    public Task BindStateAsync(long chatId, StateHistory<T> state, CancellationToken cancellationToken)
    {
        var value = stateFactoryDataCollection.FirstOrDefault(z => z.StateTypeName == state.StateType.Name);

        if (!value.CheckAny())
        {
            throw new Exception("Состояние не найдено");
        }

        return AddBindStateAsync(chatId, value?.StateTypeName, cancellationToken);
    }

    public Task UnBindStateAsync(long chatId, CancellationToken cancellationToken)
    {
        return RemoveBindStateAsync(chatId, cancellationToken);
    }

    private async Task<StateHistory<T>> GetLastStateWithMenuOrMainAsync(List<string> statesInMemoryOrEmpty, long chatId, CancellationToken cancellationToken)
    {
        while (true)
        {
            var lastState = statesInMemoryOrEmpty.LastOrDefault();

            if (!lastState.CheckAny())
            {
                await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);
                
                await UnBindStateAsync(chatId, cancellationToken);

                return CreateContextFunc();
            }

            var state = stateFactoryDataCollection.FirstOrDefault(x => x.StateTypeName == lastState);

            if (state?.MenuTypeName != null)
            {
                await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);

                await UnBindStateAsync(chatId, cancellationToken);

                return Convert(state);
            }

            statesInMemoryOrEmpty.Remove(lastState);
        }
    }
}