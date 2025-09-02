#nullable enable
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TBotPlatform.Contracts.Abstractions.Cache;

namespace TBotPlatform.Contracts.Abstractions.Builder;

public interface ICacheBuilder
{
    /// <summary>
    /// Добавляет Redis cache
    /// </summary>
    /// <param name="redisConnectionString">Строка подключения к redis</param>
    /// <param name="client">Клиент redis</param>
    /// <param name="prefix">Префикс ключей в кеше</param>
    /// <param name="tags">Тег хелсчека</param>
    /// <returns></returns>
    ICacheBuilder AddRedisCache(string redisConnectionString, ConnectionMultiplexer? client = null, string? prefix = null, string[]? tags = null);
    
    /// <summary>
    /// Добавляет кастомный кеш
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    ICacheBuilder AddCustomCache<T>()
        where T : ICacheService;

    /// <summary>
    /// Добавляет распределенную блокировку на основе кеша
    /// </summary>
    /// <returns></returns>
    ICacheBuilder AddDistributedLock();

    /// <summary>
    /// Собирает кеш
    /// </summary>
    /// <returns></returns>
    IBotPlatformBuilder Build();
}
