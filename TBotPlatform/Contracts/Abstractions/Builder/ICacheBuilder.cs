#nullable enable
using TBotPlatform.Contracts.Abstractions.Factories;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform.Contracts.Abstractions.Builder;

public interface ICacheBuilder
{
    /// <summary>
    /// Adds Redis cache <see cref="IFusionCache"/>
    /// </summary>
    /// <param name="redisConnectionString">Redis connection string</param>
    IRedisBuilder AddRedisFusionCache(string redisConnectionString);

    /// <summary>
    /// Adds in-memory cache <see cref="IFusionCache"/> without Redis
    /// </summary>
    ICacheBuilder AddMemoryFusionCache();

    /// <summary>
    /// Checks for a previously added cache <see cref="IFusionCache"/>
    /// </summary>
    ICacheBuilder CheckCustomFusionCache();

    /// <summary>
    /// Builds the cache. Adds a distributed lock based on the cache <see cref="IDistributedLockFactory"/>
    /// </summary>
    IBotPlatformBuilder Build();
}
