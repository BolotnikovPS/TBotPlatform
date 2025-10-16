using TBotPlatform.Contracts.Cache;
using TBotPlatform.Extension;
using TBotPlatform.Results;

namespace TBotPlatform.Common.Factories;

internal partial class StateFactory
{
    private static string CacheCollectionKeyName(string botName) => $"{botName}_UserStates";
    private static string CacheBindCollectionKeyName(string botName) => $"{botName}_UserStatesBind";

    private const int MaxState = 10;

    private async Task<List<string>> GetStatesInCacheOrEmpty(string botName, long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var values = await cache.GetValueFromCollection<UserStateInCache>(CacheCollectionKeyName(botName), chatId.ToString());

        if (values.IsNull())
        {
            return [];
        }

        return values.StatesTypeName.CheckAny() ? values.StatesTypeName : [];
    }

    private async Task UpdateValueState(string botName, List<string> statesInMemoryOrEmpty, long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

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

        await cache.AddValueToCollection(CacheCollectionKeyName(botName), values);
    }

    private async Task<ResultT<string>> GetBindStateName(string botName, long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var values = await cache.GetValueFromCollection<UserBindStateInCache>(CacheBindCollectionKeyName(botName), chatId.ToString());

        return values.IsNull()
            ? ResultT<string>.Failure(ErrorResult.NotFound(StateNotFound))
            : ResultT<string>.Success(values.StatesTypeName);
    }

    private async Task AddBindState(string botName, long chatId, string statesTypeName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var value = await cache.GetValueFromCollection<UserBindStateInCache>(CacheBindCollectionKeyName(botName), chatId.ToString());

        if (value.IsNotNull())
        {
            await RemoveBindState(botName, chatId, cancellationToken);
        }

        value = new()
        {
            ChatId = chatId.ToString(),
            StatesTypeName = statesTypeName,
        };

        await cache.AddValueToCollection(CacheBindCollectionKeyName(botName), value);
    }

    private Task RemoveBindState(string botName, long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return cache.RemoveValueFromCollection(CacheBindCollectionKeyName(botName), chatId.ToString());
    }
}