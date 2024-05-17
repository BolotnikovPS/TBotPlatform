using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Contracts.Cache;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Factories;

internal class StateFactory<T>(
    ICacheService cache,
    StateFactoryDataCollection stateFactoryDataCollection,
    StateFactorySettings stateFactorySettings
    ) : IStateFactory<T>
    where T : UserBase
{
    private const string CacheCollectionKeyName = "UserStates";
    private const int MaxState = 10;

    public StateHistory<T> GetStateByNameOrDefault(string nameOfState = "")
    {
        if (nameOfState?.Length == 0)
        {
            return CreateContextFunc();
        }

        var newState = stateFactoryDataCollection.FirstOrDefault(q => q.StateTypeName == nameOfState);

        return newState.CheckAny()
            ? Convert(newState)
            : CreateContextFunc();
    }

    public async Task<StateHistory<T>> GetStateByButtonsTypeOrDefaultAsync(long chatId, string buttonTypeValue, CancellationToken cancellationToken)
    {
        var newState = stateFactoryDataCollection
           .FirstOrDefault(
                q => q.ButtonsTypes.CheckAny()
                     && q.ButtonsTypes.Any(z => z == buttonTypeValue)
                );

        var statesInMemoryOrEmpty = await GetStatesInMemoryOrEmptyAsync(chatId, cancellationToken);

        if (newState?.IsInlineState == false
            && newState.StateTypeName != statesInMemoryOrEmpty?.LastOrDefault()
           )
        {
            statesInMemoryOrEmpty?.Add(newState.StateTypeName);
            await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);
        }

        return newState.CheckAny()
            ? Convert(newState)
            : CreateContextFunc();
    }

    public async Task<StateHistory<T>> GetStateByCommandsTypeOrDefaultAsync(long chatId, string commandTypeValue, CancellationToken cancellationToken)
    {
        var newState = stateFactoryDataCollection
           .FirstOrDefault(
                q => q.CommandsTypes.CheckAny()
                     && q.CommandsTypes.Any(z => z == commandTypeValue)
                );

        var statesInMemoryOrEmpty = await GetStatesInMemoryOrEmptyAsync(chatId, cancellationToken);

        if (newState?.IsInlineState == false
            && newState.StateTypeName != statesInMemoryOrEmpty?.LastOrDefault()
           )
        {
            statesInMemoryOrEmpty?.Add(newState.StateTypeName);
            await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);
        }

        return newState.CheckAny()
            ? Convert(newState)
            : CreateContextFunc();
    }

    public StateHistory<T> GetStateByTextsTypeOrDefault(long chatId, string textTypeValue)
    {
        var newState = stateFactoryDataCollection
           .FirstOrDefault(
                q => q.TextsTypes.CheckAny()
                     && q.TextsTypes.Any(z => z == textTypeValue)
                );

        return newState.CheckAny()
            ? Convert(newState)
            : CreateContextFunc();
    }

    public async Task<StateHistory<T>> GetStateMainAsync(long chatId, CancellationToken cancellationToken)
    {
        var statesInMemoryOrEmpty = await GetStatesInMemoryOrEmptyAsync(chatId, cancellationToken);
        statesInMemoryOrEmpty.Clear();
        await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);

        return CreateContextFunc();
    }

    public async Task<StateHistory<T>> GetStatePreviousOrMainAsync(long chatId, CancellationToken cancellationToken)
    {
        var statesInMemoryOrEmpty = await GetStatesInMemoryOrEmptyAsync(chatId, cancellationToken);
        if (statesInMemoryOrEmpty.Count == 0)
        {
            return CreateContextFunc();
        }

        statesInMemoryOrEmpty.RemoveAt(statesInMemoryOrEmpty.Count - 1);

        return await GetLastStateWithMenuOrMainAsync(statesInMemoryOrEmpty, chatId, cancellationToken);
    }

    public async Task<IMenuButton<T>> GetLastMenuAsync(long chatId, CancellationToken cancellationToken)
    {
        var statesInMemoryOrEmpty = await GetStatesInMemoryOrEmptyAsync(chatId, cancellationToken);
        var result = await GetLastStateWithMenuOrMainAsync(statesInMemoryOrEmpty, chatId, cancellationToken);

        return result.CheckAny() ? result.MenuState : default;
    }

    public StateHistory<T> GetLockState()
    {
        var state = stateFactoryDataCollection.FirstOrDefault(z => z.IsLockUserState);

        return Convert(state);
    }

    private async Task<List<string>> GetStatesInMemoryOrEmptyAsync(long chatId, CancellationToken cancellationToken)
    {
        var values = await cache.GetValueFromCollectionAsync<UserStateInCache>(CacheCollectionKeyName, chatId.ToString(), cancellationToken);

        if (!values.CheckAny())
        {
            return [];
        }

        return values.StatesTypeName.CheckAny() ? values.StatesTypeName : [];
    }

    private async Task UpdateValueStateAsync(List<string> statesInMemoryOrEmpty, long chatId, CancellationToken cancellationToken)
    {
        while (statesInMemoryOrEmpty.Count > MaxState)
        {
            var toRemove = statesInMemoryOrEmpty.FirstOrDefault();
            statesInMemoryOrEmpty.Remove(toRemove);
        }

        var values = new UserStateInCache
        {
            ChatId = chatId.ToString(),
            StatesTypeName = statesInMemoryOrEmpty,
        };

        await cache.AddValueToCollectionAsync(CacheCollectionKeyName, values, cancellationToken);
    }

    private async Task<StateHistory<T>> GetLastStateWithMenuOrMainAsync(List<string> statesInMemoryOrEmpty, long chatId, CancellationToken cancellationToken)
    {
        while (true)
        {
            var lastState = statesInMemoryOrEmpty.LastOrDefault();

            if (!lastState.CheckAny())
            {
                await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);
                return CreateContextFunc();
            }

            var state = stateFactoryDataCollection.FirstOrDefault(x => x.StateTypeName == lastState);

            if (state?.MenuTypeName != null)
            {
                await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);
                return Convert(state);
            }

            statesInMemoryOrEmpty.Remove(lastState);
        }
    }

    private StateHistory<T> CreateContextFunc()
    {
        var state = stateFactoryDataCollection
           .FirstOrDefault(
                q => q.CommandsTypes.CheckAny()
                     && q.CommandsTypes.Any(x => x.In("/start"))
                );

        return Convert(state!);
    }

    private StateHistory<T> Convert(StateFactoryData lazyStateHistory)
    {
        var stateType = FindType(lazyStateHistory.StateTypeName);

        if (!stateType.CheckAny())
        {
            throw new Exception("Состояние не найдено");
        }

        return new StateHistory<T>(
            stateType,
            lazyStateHistory.MenuTypeName.CheckAny()
                ? Activator.CreateInstance(FindType(lazyStateHistory.MenuTypeName)) as IMenuButton<T>
                : null,
            lazyStateHistory.IsInlineState
            );

        Type FindType(string name)
        {
            return Array.Find(
                stateFactorySettings
                   .Assembly
                   .GetTypes(),
                z => z.Name == name
                );
        }
    }
}