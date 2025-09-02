using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TBotPlatform.Common.Dependencies;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Builder;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Factories;

namespace TBotPlatform.Common.Builder;
internal class CacheBuilder(IServiceCollection serviceCollection, IBotPlatformBuilder botPlatformBuilder) : ICacheBuilder
{
    private bool IsCacheAdded { get; set; }

    public ICacheBuilder AddRedisCache(string redisConnectionString, ConnectionMultiplexer client = null, string prefix = null, string[] tags = null)
    {
        if (IsCacheAdded)
        {
            throw new InvalidOperationException("Кеш ранее был добавлен.");
        }

        serviceCollection.AddCache(redisConnectionString, client, prefix, tags);

        IsCacheAdded = true;

        return this;
    }

    public ICacheBuilder AddCustomCache<T>()
        where T : ICacheService
    {
        if (IsCacheAdded)
        {
            throw new InvalidOperationException("Кеш ранее был добавлен.");
        }

        IsCacheAdded = true;

        serviceCollection.AddCache<T>();

        return this;
    }

    public ICacheBuilder AddDistributedLock()
    {
        if (!IsCacheAdded)
        {
            throw new InvalidOperationException("Отсутствует кеш.");
        }
        
        serviceCollection.AddSingleton<IDistributedLockFactory, DistributedLockFactory>();

        return this;
    }

    public IBotPlatformBuilder Build() => botPlatformBuilder;
}
