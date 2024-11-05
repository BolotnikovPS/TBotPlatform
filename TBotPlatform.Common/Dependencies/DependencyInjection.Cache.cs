#nullable enable
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TBotPlatform.Common.Cache;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddCache<T>(this IServiceCollection services)
        where T : ICacheService
        => services
           .AddSingleton(typeof(ICacheService), typeof(T));

    public static IServiceCollection AddCache(this IServiceCollection services, string redisConnectionString, ConnectionMultiplexer? client = null, string? prefix = null, string[]? tags = null)
    {
        if (client.IsNotNull())
        {
            services.AddSingleton(new Lazy<ConnectionMultiplexer>(client!));
        }
        else
        {
            services.AddSingleton(
                _ =>
                {
                    client = ConnectionMultiplexer.Connect(redisConnectionString, Console.Out);
                    return new Lazy<ConnectionMultiplexer>(client);
                });
        }

        services
           .AddSingleton(
                _ => new CacheSettings
                {
                    CachePrefix = prefix,
                })
           .AddSingleton<ICacheService, CacheService>()
           .AddHealthCheckRedis(redisConnectionString, tags);

        return services;
    }

    public static IServiceCollection AddCacheWithDistributedLock(
        this IServiceCollection services,
        string redisConnectionString,
        ConnectionMultiplexer? client = null,
        string? prefix = null,
        string[]? tags = null
        ) => services
            .AddCache(redisConnectionString, client, prefix, tags)
            .AddSingleton<IDistributedLockFactory, DistributedLockFactory>();

    private static void AddHealthCheckRedis(this IServiceCollection services, string redisConnectionString, string[]? tags = null)
    {
        if (tags.IsNull())
        {
            return;
        }

        services
           .AddHealthChecks()
           .AddRedis(redisConnectionString, name: null, failureStatus: null, tags);
    }
}