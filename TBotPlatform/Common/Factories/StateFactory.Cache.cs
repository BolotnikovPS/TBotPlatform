using TBotPlatform.Contracts.Cache;
using TBotPlatform.Extension;
using TBotPlatform.Results.Abstractions;

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

        if (!values.IsSuccess)
        {
            return [];
        }

        return values.Value.StatesTypeName.CheckAny() ? values.Value.StatesTypeName : [];
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

    private Task<IResult<UserBindStateInCache>> GetBindStateName(string botName, long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return cache.GetValueFromCollection<UserBindStateInCache>(CacheBindCollectionKeyName(botName), chatId.ToString());
    }

    private async Task AddBindState(string botName, long chatId, string statesTypeName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var value = await cache.GetValueFromCollection<UserBindStateInCache>(CacheBindCollectionKeyName(botName), chatId.ToString());

        if (value.IsSuccess)
        {
            await RemoveBindState(botName, chatId, cancellationToken);
        }

        var cacheValue = new UserBindStateInCache()
        {
            ChatId = chatId.ToString(),
            StatesTypeName = statesTypeName,
        };

        await cache.AddValueToCollection(CacheBindCollectionKeyName(botName), cacheValue);
    }

    private Task<IResult> RemoveBindState(string botName, long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return cache.RemoveValueFromCollection(CacheBindCollectionKeyName(botName), chatId.ToString());
    }
}