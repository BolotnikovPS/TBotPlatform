using Microsoft.Extensions.Options;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Factories;

internal partial class StateFactory(ICacheService cache, IOptions<StateFactoryDataCollection> stateFactoryDataCollection, IOptions<StateFactorySettings> stateFactorySettings) : IStateFactory
{
    public StateHistory GetStateByNameOrDefault(string nameOfState = "")
    {
        if (nameOfState.IsNull())
        {
            return CreateContextFunc();
        }

        var newState = stateFactoryDataCollection.Value.FirstOrDefault(q => q.StateTypeName == nameOfState);

        return ConvertStateFactoryData(newState);
    }

    public async Task<StateHistory> GetStateByButtonsTypeOrDefaultAsync(long chatId, string buttonTypeValue, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(buttonTypeValue);

        var newState = stateFactoryDataCollection.Value
                                                 .FirstOrDefault(
                                                      q => q.ButtonsTypes.CheckAny()
                                                           && q.ButtonsTypes.Any(z => z == buttonTypeValue)
                                                      );

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmptyAsync(chatId, cancellationToken);

        var checkNewState = newState.IsNotNull()
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

    public async Task<StateHistory> GetStateByCommandsTypeOrDefaultAsync(long chatId, string commandTypeValue, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(commandTypeValue);

        var newState = stateFactoryDataCollection.Value
                                                 .FirstOrDefault(
                                                      q => q.CommandsTypes.CheckAny()
                                                           && q.CommandsTypes.Any(z => z == commandTypeValue)
                                                      );

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmptyAsync(chatId, cancellationToken);

        var checkNewState = newState.IsNotNull()
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

    public StateHistory GetStateByTextsTypeOrDefault(long chatId, string textTypeValue)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(textTypeValue);

        var newState = stateFactoryDataCollection.Value
                                                 .FirstOrDefault(
                                                      q => q.TextsTypes.CheckAny()
                                                           && q.TextsTypes.Any(z => z == textTypeValue)
                                                      );

        return ConvertStateFactoryData(newState);
    }

    public async Task<StateHistory> GetStateMainAsync(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmptyAsync(chatId, cancellationToken);
        statesInMemoryOrEmpty.Clear();
        await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);

        await UnBindStateAsync(chatId, cancellationToken);

        return CreateContextFunc();
    }

    public async Task<StateHistory> GetStatePreviousOrMainAsync(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmptyAsync(chatId, cancellationToken);
        if (statesInMemoryOrEmpty.Count == 0)
        {
            return CreateContextFunc();
        }

        statesInMemoryOrEmpty.RemoveAt(statesInMemoryOrEmpty.Count - 1);

        return await GetLastStateWithMenuOrMainAsync(statesInMemoryOrEmpty, chatId, cancellationToken);
    }

    public async Task<StateHistory> GetLastStateWithMenuAsync(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmptyAsync(chatId, cancellationToken);
        var result = await GetLastStateWithMenuOrMainAsync(statesInMemoryOrEmpty, chatId, cancellationToken);

        return result.IsNotNull() ? result : default;
    }

    public StateHistory GetLockState()
    {
        var state = stateFactoryDataCollection.Value.FirstOrDefault(z => z.IsLockUserState);

        return state.IsNotNull() ? Convert(state) : default;
    }

    public StateHistory GetRegistrationState()
    {
        var state = stateFactoryDataCollection.Value.FirstOrDefault(z => z.IsRegistrationState);

        return state.IsNotNull() ? Convert(state) : default;
    }

    public async Task<StateHistory> GetBindStateOrNullAsync(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var value = await GetBindStateNameAsync(chatId, cancellationToken);

        var state = stateFactoryDataCollection.Value.FirstOrDefault(z => z.StateTypeName == value);

        return value.IsNotNull() ? Convert(state) : null;
    }

    public Task BindStateAsync(long chatId, StateHistory state, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(state);

        var value = stateFactoryDataCollection.Value.FirstOrDefault(z => z.StateTypeName == state.StateType.Name);

        if (value.IsNull())
        {
            throw new("Состояние не найдено");
        }

        return AddBindStateAsync(chatId, value?.StateTypeName, cancellationToken);
    }

    public Task UnBindStateAsync(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        return RemoveBindStateAsync(chatId, cancellationToken);
    }

    public async Task<bool> HasBindStateAsync(long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var value = await GetBindStateNameAsync(chatId, cancellationToken);

        return value.CheckAny();
    }

    private async Task<StateHistory> GetLastStateWithMenuOrMainAsync(List<string> statesInMemoryOrEmpty, long chatId, CancellationToken cancellationToken)
    {
        var lastStateToRemove = statesInMemoryOrEmpty.LastOrDefault();

        if (lastStateToRemove.IsNull())
        {
            await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);

            await UnBindStateAsync(chatId, cancellationToken);

            return CreateContextFunc();
        }

        while (true)
        {
            var lastState = statesInMemoryOrEmpty.LastOrDefault();

            if (lastState.IsNull())
            {
                await UpdateValueStateAsync(statesInMemoryOrEmpty, chatId, cancellationToken);

                await UnBindStateAsync(chatId, cancellationToken);

                return CreateContextFunc();
            }

            var state = stateFactoryDataCollection.Value.FirstOrDefault(x => x.StateTypeName == lastState);

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