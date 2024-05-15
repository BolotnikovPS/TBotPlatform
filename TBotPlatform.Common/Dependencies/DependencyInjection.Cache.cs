using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TBotPlatform.Common.Cache;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddCache<T>(this IServiceCollection services)
        where T : ICacheService
    {
        services
           .AddSingleton(typeof(ICacheService), typeof(T));

        return services;
    }

    public static IServiceCollection AddCache(this IServiceCollection services, string redisConnectionString, string prefix, string[] tags = null)
    {
        services
           .AddSingleton(
                _ => new CacheSettings
                {
                    CachePrefix = prefix,
                })
           .AddSingleton(
                _ =>
                {
                    var client = ConnectionMultiplexer.Connect(redisConnectionString, Console.Out);
                    return new Lazy<ConnectionMultiplexer>(client);
                })
           .AddSingleton<ICacheService, CacheService>()
           .AddHealthCheckRedis(redisConnectionString, tags);

        return services;
    }

    private static void AddHealthCheckRedis(this IServiceCollection services, string redisConnectionString, string[] tags = null)
    {
        if (tags.IsNull())
        {
            return;
        }

        services
           .AddHealthChecks()
           .AddRedis(redisConnectionString, null, null, tags);
    }
}