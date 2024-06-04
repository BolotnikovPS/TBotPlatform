using TBotPlatform.Contracts.Cache;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Factories;

internal partial class StateFactory<T>
{
    private async Task<List<string>> GetStatesInCacheOrEmptyAsync(long chatId, CancellationToken cancellationToken)
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
}