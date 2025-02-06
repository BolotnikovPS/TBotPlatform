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

    private async Task<List<string>> GetStatesInCacheOrEmpty(long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var values = await cache.GetValueFromCollection<UserStateInCache>(CacheCollectionKeyName, chatId.ToString());

        if (values.IsNull())
        {
            return [];
        }

        return values.StatesTypeName.CheckAny() ? values.StatesTypeName : [];
    }

    private async Task UpdateValueState(List<string> statesInMemoryOrEmpty, long chatId, CancellationToken cancellationToken)
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

        await cache.AddValueToCollection(CacheCollectionKeyName, values);
    }

    private async Task<string> GetBindStateName(long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var values = await cache.GetValueFromCollection<UserBindStateInCache>(CacheBindCollectionKeyName, chatId.ToString());

        return values.IsNotNull()
            ? values.StatesTypeName
            : default;
    }

    private async Task AddBindState(long chatId, string statesTypeName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var value = await cache.GetValueFromCollection<UserBindStateInCache>(CacheBindCollectionKeyName, chatId.ToString());

        if (value.IsNotNull())
        {
            await RemoveBindState(chatId, cancellationToken);
        }

        value = new()
        {
            ChatId = chatId.ToString(),
            StatesTypeName = statesTypeName,
        };

        await cache.AddValueToCollection(CacheBindCollectionKeyName, value);
    }

    private Task RemoveBindState(long chatId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return cache.RemoveValueFromCollection(CacheBindCollectionKeyName, chatId.ToString());
    }
}