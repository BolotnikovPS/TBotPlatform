using TBotPlatform.Contracts.Cache;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Factories;

internal partial class StateFactory<T>
{
    private const string CacheCollectionKeyName = "UserStates";
    private const string CacheBindCollectionKeyName = "UserStatesBind";
    private const int MaxState = 10;

    private async Task<List<string>> GetStatesInCacheOrEmptyAsync(long chatId, CancellationToken cancellationToken)
    {
        var values = await cache.GetValueFromCollectionAsync<UserStateInCache>(CacheCollectionKeyName, chatId.ToString(), cancellationToken);

        if (values.IsNull())
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

    private async Task<string> GetBindStateNameAsync(long chatId, CancellationToken cancellationToken)
    {
        var values = await cache.GetValueFromCollectionAsync<UserBindStateInCache>(CacheBindCollectionKeyName, chatId.ToString(), cancellationToken);

        return values.IsNotNull()
            ? values.StatesTypeName
            : default;
    }

    private async Task AddBindStateAsync(long chatId, string statesTypeName, CancellationToken cancellationToken)
    {
        var value = await cache.GetValueFromCollectionAsync<UserBindStateInCache>(CacheBindCollectionKeyName, chatId.ToString(), cancellationToken);

        if (value.IsNotNull())
        {
            await RemoveBindStateAsync(chatId, cancellationToken);
        }

        value = new()
        {
            ChatId = chatId.ToString(),
            StatesTypeName = statesTypeName,
        };

        await cache.AddValueToCollectionAsync(CacheBindCollectionKeyName, value, cancellationToken);
    }

    private Task RemoveBindStateAsync(long chatId, CancellationToken cancellationToken)
    {
        return cache.RemoveValueFromCollectionAsync(CacheBindCollectionKeyName, chatId.ToString(), cancellationToken);
    }
}