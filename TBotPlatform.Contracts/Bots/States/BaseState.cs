using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Cache;
using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots.States;

public abstract class BaseState(ICacheService cacheService)
{
    protected readonly ICacheService CacheService = cacheService;

    protected string StateName
        => GetType().Name;

    protected async Task<T> GetValueStateOrNullFromCacheAsync<T>(int userId, CancellationToken cancellationToken)
    {
        CacheServiceCheck();

        var result = await CacheService.GetValueFromCollectionAsync<BaseStateInCache<T>>(GetCacheCollectionName(userId), GetCacheKeyName(), cancellationToken);

        return result.IsNotNull()
            ? result.Value
            : default;
    }

    protected Task AddValueStateInCacheAsync<T>(int userId, T value, CancellationToken cancellationToken)
    {
        CacheServiceCheck();

        var data = new BaseStateInCache<T>
        {
            Value = value,
            Key = GetCacheKeyName(),
        };

        return CacheService.AddValueToCollectionAsync(GetCacheCollectionName(userId), data, cancellationToken);
    }

    protected Task RemoveValueStateInCacheAsync(int userId, CancellationToken cancellationToken)
    {
        CacheServiceCheck();

        return CacheService.RemoveValueFromCollectionAsync(GetCacheCollectionName(userId), GetCacheKeyName(), cancellationToken);
    }

    protected Task RemoveValuesInCacheAsync(int userId, CancellationToken cancellationToken)
    {
        CacheServiceCheck();

        return CacheService.RemoveCollectionAsync(GetCacheCollectionName(userId), cancellationToken);
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