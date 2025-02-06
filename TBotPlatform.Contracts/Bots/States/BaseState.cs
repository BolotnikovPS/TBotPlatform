using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Cache;
using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots.States;

public abstract class BaseState(ICacheService cacheService)
{
    protected readonly ICacheService CacheService = cacheService;

    protected string StateName
        => GetType().Name;

    protected virtual async Task<T> GetValueStateOrNullFromCache<T>(int userId)
    {
        CacheServiceCheck();

        var result = await CacheService.GetValueFromCollection<BaseStateInCache<T>>(GetCacheCollectionName(userId), GetCacheKeyName());

        return result.IsNotNull()
            ? result.Value
            : default;
    }

    protected virtual Task AddValueStateInCache<T>(int userId, T value)
    {
        CacheServiceCheck();

        var data = new BaseStateInCache<T>
        {
            Value = value,
            Key = GetCacheKeyName(),
        };

        return CacheService.AddValueToCollection(GetCacheCollectionName(userId), data);
    }

    protected virtual Task RemoveValueStateInCache(int userId)
    {
        CacheServiceCheck();

        return CacheService.RemoveValueFromCollection(GetCacheCollectionName(userId), GetCacheKeyName());
    }

    protected virtual Task RemoveValuesInCache(int userId)
    {
        CacheServiceCheck();

        return CacheService.RemoveCollection(GetCacheCollectionName(userId));
    }

    private void CacheServiceCheck()
    {
        if (CacheService.IsNotNull())
        {
            return;
        }

        throw new NotImplementedException($"Для состояния {StateName} не реализован интерфейс {nameof(ICacheService)}");
    }

    private string GetCacheKeyName()
        => StateName;

    private static string GetCacheCollectionName(int userId)
        => $"Temp_{userId}";
}