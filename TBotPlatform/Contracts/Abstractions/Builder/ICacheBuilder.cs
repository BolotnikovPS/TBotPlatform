#nullable enable
using TBotPlatform.Contracts.Abstractions.Cache;

namespace TBotPlatform.Contracts.Abstractions.Builder;

public interface ICacheBuilder
{
    /// <summary>
    /// Добавляет Redis cache
    /// </summary>
    /// <param name="redisConnectionString">Строка подключения к redis</param>
    /// <returns></returns>
    IRedisBuilder AddRedisCache(string redisConnectionString);
    
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
