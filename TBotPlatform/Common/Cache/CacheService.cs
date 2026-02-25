using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Cache;

internal class CacheService(ILogger<CacheService> logger, Lazy<ConnectionMultiplexer> lazyMultiplexer, string cachePrefix) : ICacheService
{
    private IDatabase DbCache => lazyMultiplexer.Value.GetDatabase();

    public Task AddValueToCollection(string collection, IKeyInCache value) => DbCache.HashSetAsync(CreateCollectionName(collection), value.Key, value.ToJson());

    public async Task<T> GetValueFromCollection<T>(string collection, string key)
        where T : IKeyInCache
    {
        var redisValue = await DbCache.HashGetAsync(CreateCollectionName(collection), key);

        return DeserializeObject<T>(redisValue);
    }

    public async Task<List<T>> GetAllValueFromCollection<T>(string collection)
        where T : IKeyInCache
    {
        var redisValue = await DbCache.HashGetAllAsync(CreateCollectionName(collection));

        return redisValue.Length > 0
            ? [.. redisValue.Select(item => DeserializeObject<T>(item.Value)).Where(item => item.IsNotNull())]
            : [];
    }

    public Task RemoveValueFromCollection(string collection, string key) => DbCache.HashDeleteAsync(CreateCollectionName(collection), key);

    public Task RemoveCollection(string collection) => DbCache.KeyDeleteAsync(CreateCollectionName(collection));

    public async Task<T> GetValue<T>(string key)
        where T : IKeyInCache
    {
        var value = await DbCache.StringGetAsync(CreateKeyName(key));

        return DeserializeObject<T>(value);
    }

    public Task<bool> SetValue(IKeyInCache value, TimeSpan expiryTime) => DbCache.StringSetAsync(CreateKeyName(value.Key), value.ToJson(), expiryTime);

    public Task<bool> SetValue(IKeyInCache value) => DbCache.StringSetAsync(CreateKeyName(value.Key), value.ToJson());

    public Task<bool> RemoveValue(string key) => DbCache.KeyDeleteAsync(CreateKeyName(key));

    public Task<bool> KeyExists(string key) => DbCache.KeyExistsAsync(CreateKeyName(key));

    private T DeserializeObject<T>(RedisValue? value)
        where T : IKeyInCache
    {
        if (value.IsNull())
        {
            return default;
        }

        try
        {
            return value.ToString().FromJson<T>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка десериализации объекта {value}", value.ToString());

            return default;
        }
    }

    private string CreateCollectionName(string collection) => cachePrefix.CheckAny() ? $"{cachePrefix}_{collection}" : collection;

    private string CreateKeyName(string key) => cachePrefix.CheckAny() ? $"{cachePrefix}_{key}" : key;
}