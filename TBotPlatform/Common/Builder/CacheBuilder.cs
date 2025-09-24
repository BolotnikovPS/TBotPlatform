using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Builder;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Factories;

namespace TBotPlatform.Common.Builder;

internal class CacheBuilder(IServiceCollection serviceCollection, IBotPlatformBuilder botPlatformBuilder) : ICacheBuilder
{
    private bool IsCacheAdded { get; set; }

    public IRedisBuilder AddRedisCache(string redisConnectionString)
    {
        if (IsCacheAdded)
        {
            throw new InvalidOperationException("Кеш ранее был добавлен.");
        }

        IsCacheAdded = true;

        return new RedisBuilder(serviceCollection, this, redisConnectionString);
    }

    public ICacheBuilder AddCustomCache<T>()
        where T : ICacheService
    {
        if (IsCacheAdded)
        {
            throw new InvalidOperationException("Кеш ранее был добавлен.");
        }

        IsCacheAdded = true;

        serviceCollection.AddSingleton(typeof(ICacheService), typeof(T));

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
