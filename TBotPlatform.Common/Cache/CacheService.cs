﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Cache;

internal class CacheService(ILogger<CacheService> logger, Lazy<ConnectionMultiplexer> lazyMultiplexer, IOptions<CacheSettings> cacheSettings) : ICacheService
{
    private IDatabase DbCache => lazyMultiplexer.Value.GetDatabase();

    Task ICacheService.AddValueToCollection(string collection, IKeyInCache value) => DbCache.HashSetAsync(CreateCollectionName(collection), value.Key, value.ToJson());

    async Task<T> ICacheService.GetValueFromCollection<T>(string collection, string key)
    {
        var redisValue = await DbCache.HashGetAsync(CreateCollectionName(collection), key);

        return DeserializeObject<T>(redisValue);
    }

    async Task<List<T>> ICacheService.GetAllValueFromCollection<T>(string collection)
    {
        var redisValue = await DbCache.HashGetAllAsync(CreateCollectionName(collection));

        return redisValue.IsNotNull()
            ? redisValue.Select(item => DeserializeObject<T>(item.Value)).ToList()
            : null;
    }

    Task ICacheService.RemoveValueFromCollection(string collection, string key) => DbCache.HashDeleteAsync(CreateCollectionName(collection), key);

    Task ICacheService.RemoveCollection(string collection) => DbCache.KeyDeleteAsync(CreateCollectionName(collection));

    async Task<T> ICacheService.GetValue<T>(string key)
    {
        var value = await DbCache.StringGetAsync(CreateKeyName(key));

        return DeserializeObject<T>(value);
    }

    Task<bool> ICacheService.SetValue(IKeyInCache value, TimeSpan expiryTime) => DbCache.StringSetAsync(CreateKeyName(value.Key), value.ToJson(), expiryTime);

    Task<bool> ICacheService.SetValue(IKeyInCache value) => DbCache.StringSetAsync(CreateKeyName(value.Key), value.ToJson());

    async Task<bool> ICacheService.RemoveValue(string key)
    {
        if (await DbCache.KeyExistsAsync(CreateKeyName(key)))
        {
            return await DbCache.KeyDeleteAsync(CreateKeyName(key));
        }

        return false;
    }

    Task<bool> ICacheService.KeyExists(string key) => DbCache.KeyExistsAsync(CreateKeyName(key));

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
            logger.LogError(ex, "Ошибка десериализации объекта {value}", value);

            return default;
        }
    }

    private string CreateCollectionName(string collection) => cacheSettings.Value.CachePrefix.IsNotNull() ? $"{cacheSettings.Value.CachePrefix}_{collection}" : collection;

    private string CreateKeyName(string key) => cacheSettings.Value.CachePrefix.IsNotNull() ? $"{cacheSettings.Value.CachePrefix}_{key}" : key;
}