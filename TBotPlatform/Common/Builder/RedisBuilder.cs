#nullable enable

using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Contracts.Abstractions.Builder;
using TBotPlatform.Extension;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.NewtonsoftJson;

namespace TBotPlatform.Common.Builder;

internal class RedisBuilder(IServiceCollection serviceCollection, ICacheBuilder cacheBuilder, string redisConnectionString) : IRedisBuilder
{
    private string? _prefix;
    private string[]? _tags;
    private string? _healthName;

    public IRedisBuilder AddPrefix(string prefix)
    {
        if (_prefix.IsNotNull())
        {
            throw new InvalidOperationException("Префикс добавлен ранее.");
        }

        _prefix = prefix;

        return this;
    }

    public IRedisBuilder AddHealthTags(string[] tags)
    {
        if (_tags.IsNotNull())
        {
            throw new InvalidOperationException("Тег хелсчека добавлен ранее.");
        }

        _tags = tags;

        return this;
    }

    public IRedisBuilder AddHealthName(string healthName)
    {
        if (_healthName.IsNotNull())
        {
            throw new InvalidOperationException("Название хелсчека добавлено ранее.");
        }

        _healthName = healthName;

        return this;
    }

    public ICacheBuilder Build()
    {
        serviceCollection
            .AddFusionCache()
            .WithCacheKeyPrefix(_prefix)
            .WithOptions(options =>
            {
                options.DistributedCacheCircuitBreakerDuration = TimeSpan.FromSeconds(2);
            })
            .WithDefaultEntryOptions(new FusionCacheEntryOptions
            {
                Duration = TimeSpan.FromMinutes(1),
                IsFailSafeEnabled = true,
                FailSafeMaxDuration = TimeSpan.FromHours(2),
                FailSafeThrottleDuration = TimeSpan.FromSeconds(30),
                EagerRefreshThreshold = 0.9f,
                FactorySoftTimeout = TimeSpan.FromMilliseconds(100),
                FactoryHardTimeout = TimeSpan.FromMilliseconds(1500),
                DistributedCacheSoftTimeout = TimeSpan.FromSeconds(1),
                DistributedCacheHardTimeout = TimeSpan.FromSeconds(2),
                AllowBackgroundDistributedCacheOperations = true,
                JitterMaxDuration = TimeSpan.FromSeconds(2),
            })
            .WithDistributedCache(_ =>
            {
                var options = new RedisCacheOptions { Configuration = redisConnectionString };

                return new RedisCache(options);
            })
            .WithSerializer(new FusionCacheNewtonsoftJsonSerializer());

        serviceCollection
           .AddHealthChecks()
           .AddRedis(redisConnectionString, _healthName, tags: _tags, timeout: TimeSpan.FromSeconds(3));

        return cacheBuilder;
    }
}
