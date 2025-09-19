using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TBotPlatform.Common.Contracts;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Extension;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Common.Factories;

internal partial class StateFactory(ICacheService cache, IOptions<StateFactorySettings> stateFactorySettings, IServiceProvider serviceProvider) : IStateFactory
{
    private const string StateNotFound = "Состояние не найдено";
    
    public bool HasState(string botName, string nameOfState) => GetStateFactoryDataCollection(botName).Any(q => q.StateTypeName == nameOfState);

    public bool HasState(string botName, string[] nameOfStates) => GetStateFactoryDataCollection(botName).Any(q => nameOfStates.Any(z => z == q.StateTypeName));

    public IResult<StateHistory> GetStateByNameOrDefault(string botName, string nameOfState = "")
    {
        if (nameOfState.IsNull())
        {
            return CreateContextFunc(botName);
        }

        var newState = GetStateFactoryDataCollection(botName).FirstOrDefault(q => q.StateTypeName == nameOfState);

        return ConvertStateFactoryData(botName, newState);
    }

    public async Task<IResult<StateHistory>> GetStateByButtonsTypeOrDefault(string botName, long chatId, string buttonTypeValue, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(buttonTypeValue);

        var newState = GetStateFactoryDataCollection(botName).FirstOrDefault(
            q => q.ButtonsTypes.CheckAny()
                 && q.ButtonsTypes.Any(z => z == buttonTypeValue)
            );

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmpty(botName, chatId, cancellationToken);

        var checkNewState = newState.IsNotNull()
                            && !newState!.IsInlineState
                            && !newState.StateTypeName.In(statesInMemoryOrEmpty?.LastOrDefault());

        if (!checkNewState)
        {
            return ConvertStateFactoryData(botName, newState);
        }

        statesInMemoryOrEmpty?.Add(newState.StateTypeName);
        await UpdateValueState(botName, statesInMemoryOrEmpty, chatId, cancellationToken);

        await UnBindState(botName, chatId, cancellationToken);

        return ConvertStateFactoryData(botName, newState);
    }

    public async Task<IResult<StateHistory>> GetStateByCommandsTypeOrDefault(string botName, long chatId, string commandTypeValue, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(commandTypeValue);

        var newState = GetStateFactoryDataCollection(botName)
           .FirstOrDefault(
                q => q.CommandsTypes.CheckAny()
                     && q.CommandsTypes.Any(z => z == commandTypeValue)
                );

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmpty(botName, chatId, cancellationToken);

        var checkNewState = newState.IsNotNull()
                            && !newState!.IsInlineState
                            && !newState.StateTypeName.In(statesInMemoryOrEmpty?.LastOrDefault());

        if (!checkNewState)
        {
            return ConvertStateFactoryData(botName, newState);
        }

        statesInMemoryOrEmpty?.Add(newState.StateTypeName);
        await UpdateValueState(botName, statesInMemoryOrEmpty, chatId, cancellationToken);

        await UnBindState(botName, chatId, cancellationToken);

        return ConvertStateFactoryData(botName, newState);
    }

    public IResult<StateHistory> GetStateByTextsTypeOrDefault(string botName, long chatId, string textTypeValue)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(textTypeValue);

        var newState = GetStateFactoryDataCollection(botName)
           .FirstOrDefault(
                q => q.TextsTypes.CheckAny()
                     && q.TextsTypes.Any(z => z == textTypeValue)
                );

        return ConvertStateFactoryData(botName, newState);
    }

    public async Task<IResult<StateHistory>> GetStateMain(string botName, long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmpty(botName, chatId, cancellationToken);
        statesInMemoryOrEmpty.Clear();
        await UpdateValueState(botName, statesInMemoryOrEmpty, chatId, cancellationToken);

        await UnBindState(botName, chatId, cancellationToken);

        return CreateContextFunc(botName);
    }

    public async Task<IResult<StateHistory>> GetStatePreviousOrMain(string botName, long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmpty(botName, chatId, cancellationToken);
        if (statesInMemoryOrEmpty.Count == 0)
        {
            return CreateContextFunc(botName);
        }

        statesInMemoryOrEmpty.RemoveAt(statesInMemoryOrEmpty.Count - 1);

        return await GetLastStateWithMenuOrMain(botName, statesInMemoryOrEmpty, chatId, cancellationToken);
    }

    public async Task<IResult<StateHistory>> GetLastStateWithMenu(string botName, long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var statesInMemoryOrEmpty = await GetStatesInCacheOrEmpty(botName, chatId, cancellationToken);
        var result = await GetLastStateWithMenuOrMain(botName, statesInMemoryOrEmpty, chatId, cancellationToken);

        return result.IsNotNull() ? result : ResultT<StateHistory>.Failure(ErrorResult.NotFound(StateNotFound));
    }

    public IResult<StateHistory> LockState(string botName) => GetStateHistory(botName, z => z.IsLockUserState);

    public IResult<StateHistory> RegistrationState(string botName) => GetStateHistory(botName, z => z.IsRegistrationState);

    public async Task<IResult<StateHistory>> GetBindStateOrNull(string botName, long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var value = await GetBindStateName(botName, chatId, cancellationToken);

        var state = GetStateFactoryDataCollection(botName).FirstOrDefault(z => z.StateTypeName == value);

        return value.IsNotNull() ? Convert(state) : ResultT<StateHistory>.Failure(ErrorResult.NotFound(StateNotFound));
    }

    public async Task<IResult> BindState(string botName, long chatId, StateHistory state, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);
        ArgumentNullException.ThrowIfNull(state);

        var value = GetStateFactoryDataCollection(botName).FirstOrDefault(z => z.StateTypeName == state.StateType.Name);

        if (value.IsNull())
        {
            Result.Failure(ErrorResult.NotFound(StateNotFound));
        }

        await AddBindState(botName, chatId, value?.StateTypeName, cancellationToken);

        return Result.Success();
    }

    public async Task<IResult> UnBindState(string botName, long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        await RemoveBindState(botName, chatId, cancellationToken);

        return Result.Success();
    }

    public async Task<bool> HasBindState(string botName, long chatId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(chatId);

        var value = await GetBindStateName(botName, chatId, cancellationToken);

        return value.CheckAny();
    }

    private async Task<IResult<StateHistory>> GetLastStateWithMenuOrMain(string botName, List<string> statesInMemoryOrEmpty, long chatId, CancellationToken cancellationToken)
    {
        var lastStateToRemove = statesInMemoryOrEmpty.LastOrDefault();

        if (lastStateToRemove.IsNull())
        {
            await UpdateValueState(botName, statesInMemoryOrEmpty, chatId, cancellationToken);

            await UnBindState(botName, chatId, cancellationToken);

            return CreateContextFunc(botName);
        }

        while (true)
        {
            var lastState = statesInMemoryOrEmpty.LastOrDefault();

            if (lastState.IsNull())
            {
                await UpdateValueState(botName, statesInMemoryOrEmpty, chatId, cancellationToken);

                await UnBindState(botName, chatId, cancellationToken);

                return CreateContextFunc(botName);
            }

            var state = GetStateFactoryDataCollection(botName).FirstOrDefault(x => x.StateTypeName == lastState);

            if (state?.MenuTypeName != null)
            {
                await UpdateValueState(botName, statesInMemoryOrEmpty, chatId, cancellationToken);

                await UnBindState(botName, chatId, cancellationToken);

                return Convert(state);
            }

            statesInMemoryOrEmpty.Remove(lastState);
        }
    }

    private ResultT<StateHistory> GetStateHistory(string botName, Func<StateFactoryData, bool> request)
    {
        var state = GetStateFactoryDataCollection(botName).FirstOrDefault(request);

        return state.IsNotNull() ? Convert(state) : ResultT<StateHistory>.Failure(ErrorResult.NotFound(StateNotFound));
    }

    private StateFactoryDataCollection GetStateFactoryDataCollection(string botName) => serviceProvider.GetRequiredKeyedService<StateFactoryDataCollection>(botName);
}