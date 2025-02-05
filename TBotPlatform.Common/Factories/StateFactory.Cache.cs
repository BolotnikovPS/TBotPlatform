using TBotPlatform.Contracts.Cache;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Factories;

internal partial class StateFactory
{
    private string CacheCollectionKeyName { get; set; } = "UserStates";
    private string CacheBindCollectionKeyName { get; set; } = "UserStatesBind";

    private const int MaxState = 10;

    internal void SetCacheKeyPrefix(string prefix)
    {
        CacheCollectionKeyName = $"{prefix}_{CacheCollectionKeyName}";
        CacheBindCollectionKeyName = $"{prefix}_{CacheBindCollectionKeyName}";
    }

    private async Task<List<string>> GetStatesInCacheOrEmptyAsync(long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var values = await cache.GetValueFromCollectionAsync<UserStateInCache>(CacheCollectionKeyName, chatId.ToString());

        if (values.IsNull())
        {
            return [];
        }

        return values.StatesTypeName.CheckAny() ? values.StatesTypeName : [];
    }

    private async Task UpdateValueStateAsync(List<string> statesInMemoryOrEmpty, long chatId, CancellationToken cancellationToken)
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

        await cache.AddValueToCollectionAsync(CacheCollectionKeyName, values);
    }

    private async Task<string> GetBindStateNameAsync(long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var values = await cache.GetValueFromCollectionAsync<UserBindStateInCache>(CacheBindCollectionKeyName, chatId.ToString());

        return values.IsNotNull()
            ? values.StatesTypeName
            : default;
    }

    private async Task AddBindStateAsync(long chatId, string statesTypeName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var value = await cache.GetValueFromCollectionAsync<UserBindStateInCache>(CacheBindCollectionKeyName, chatId.ToString());

        if (value.IsNotNull())
        {
            await RemoveBindStateAsync(chatId, cancellationToken);
        }

        value = new()
        {
            ChatId = chatId.ToString(),
            StatesTypeName = statesTypeName,
        };

        await cache.AddValueToCollectionAsync(CacheBindCollectionKeyName, value);
    }

    private Task RemoveBindStateAsync(long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return cache.RemoveValueFromCollectionAsync(CacheBindCollectionKeyName, chatId.ToString());
    }
}