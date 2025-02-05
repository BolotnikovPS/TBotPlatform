using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Cache;
using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots.States;

public abstract class BaseState(ICacheService cacheService)
{
    protected readonly ICacheService CacheService = cacheService;

    protected string StateName
        => GetType().Name;

    protected virtual async Task<T> GetValueStateOrNullFromCacheAsync<T>(int userId)
    {
        CacheServiceCheck();

        var result = await CacheService.GetValueFromCollectionAsync<BaseStateInCache<T>>(GetCacheCollectionName(userId), GetCacheKeyName());

        return result.IsNotNull()
            ? result.Value
            : default;
    }

    protected virtual Task AddValueStateInCacheAsync<T>(int userId, T value)
    {
        CacheServiceCheck();

        var data = new BaseStateInCache<T>
        {
            Value = value,
            Key = GetCacheKeyName(),
        };

        return CacheService.AddValueToCollectionAsync(GetCacheCollectionName(userId), data);
    }

    protected virtual Task RemoveValueStateInCacheAsync(int userId)
    {
        CacheServiceCheck();

        return CacheService.RemoveValueFromCollectionAsync(GetCacheCollectionName(userId), GetCacheKeyName());
    }

    protected virtual Task RemoveValuesInCacheAsync(int userId)
    {
        CacheServiceCheck();

        return CacheService.RemoveCollectionAsync(GetCacheCollectionName(userId));
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