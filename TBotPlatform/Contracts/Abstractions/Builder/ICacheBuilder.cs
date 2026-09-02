#nullable enable
using TBotPlatform.Contracts.Abstractions.Factories;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform.Contracts.Abstractions.Builder;

public interface ICacheBuilder
{
    /// <summary>
    /// Добавляет Redis cache <see cref="IFusionCache"/>
    /// </summary>
    /// <param name="redisConnectionString">Строка подключения к redis</param>
    /// <returns></returns>
    IRedisBuilder AddRedisFusionCache(string redisConnectionString);

    /// <summary>
    /// Проверяет наличие ранее добавленного кеша <see cref="IFusionCache"/>
    /// </summary>
    /// <returns></returns>
    ICacheBuilder CheckCustomFusionCache();

    /// <summary>
    /// Собирает кеш. Добавляет распределенную блокировку на основе кеша <see cref="IDistributedLockFactory"/>
    /// </summary>
    /// <returns></returns>
    IBotPlatformBuilder Build();
}
