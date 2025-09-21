#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TBotPlatform.Common.Cache;
using TBotPlatform.Contracts.Abstractions.Builder;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Builder;

internal class RedisBuilder(IServiceCollection serviceCollection, ICacheBuilder cacheBuilder, string redisConnectionString) : IRedisBuilder
{
    private ConnectionMultiplexer? _client;
    private string? _prefix;
    private string[]? _tags;
    private string? _healthName;
    private HealthStatus? _healthStatus;

    public IRedisBuilder AddMyRedisConnectionMultiplexer(ConnectionMultiplexer client)
    {
        _client = client;

        return this;
    }

    public IRedisBuilder AddPrefix(string prefix)
    {
        _prefix = prefix;

        return this;
    }

    public IRedisBuilder AddTags(string[] tags)
    {
        _tags = tags;

        return this;
    }

    public IRedisBuilder AddHealthName(string healthName)
    {
        _healthName = healthName;

        return this;
    }

    public IRedisBuilder AddFailureHealthStatus(HealthStatus healthStatus)
    {
        _healthStatus = healthStatus;

        return this;
    }

    public ICacheBuilder Build()
    {
        if (_client.IsNotNull())
        {
            serviceCollection.AddSingleton(new Lazy<ConnectionMultiplexer>(_client!));
        }
        else
        {
            serviceCollection.AddSingleton(s =>
            {
                var loggerFactory = s.GetRequiredService<ILoggerFactory>();
                var loggerWriter = new CacheLoggerTextWriter(loggerFactory.CreateLogger<ConnectionMultiplexer>());

                _client = ConnectionMultiplexer.Connect(redisConnectionString, loggerWriter);

                return new Lazy<ConnectionMultiplexer>(_client);
            });
        }

        serviceCollection
           .AddSingleton<ICacheService>(s =>
           {
               var loggerFactory = s.GetRequiredService<ILoggerFactory>();
               var lazyConnectionMultiplexer = s.GetRequiredService<Lazy<ConnectionMultiplexer>>();

               return new CacheService(loggerFactory.CreateLogger<CacheService>(), lazyConnectionMultiplexer, _prefix);
           });

        if (_tags.IsNotNull())
        {
            serviceCollection
               .AddHealthChecks()
               .AddRedis(redisConnectionString, _healthName, _healthStatus, _tags);
        }

        return cacheBuilder;
    }
}
