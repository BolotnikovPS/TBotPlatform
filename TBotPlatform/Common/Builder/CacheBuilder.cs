using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Builder;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Extension;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.NewtonsoftJson;

namespace TBotPlatform.Common.Builder;

internal class CacheBuilder(IServiceCollection serviceCollection, IBotPlatformBuilder botPlatformBuilder) : ICacheBuilder
{
    private bool IsCacheAdded { get; set; }
    public IRedisBuilder AddRedisFusionCache(string redisConnectionString)
    {
        if (IsCacheAdded)
        {
            throw new InvalidOperationException("Кеш ранее был добавлен.");
        }

        if (redisConnectionString.IsNull())
        {
            throw new InvalidOperationException("Строка подключения пуста.");
        }

        IsCacheAdded = true;

        return new RedisBuilder(serviceCollection, this, redisConnectionString);
    }

    public ICacheBuilder AddMemoryFusionCache()
    {
        if (IsCacheAdded)
        {
            throw new InvalidOperationException("Кеш ранее был добавлен.");
        }

        IsCacheAdded = true;

        serviceCollection
           .AddFusionCache()
           .WithDefaultEntryOptions(new FusionCacheEntryOptions
           {
               Duration = TimeSpan.FromMinutes(1),
               IsFailSafeEnabled = true,
               FailSafeMaxDuration = TimeSpan.FromHours(2),
               FailSafeThrottleDuration = TimeSpan.FromSeconds(30),
           })
           .WithSerializer(new FusionCacheNewtonsoftJsonSerializer());

        return this;
    }

    public ICacheBuilder CheckCustomFusionCache()
    {
        if (IsCacheAdded)
        {
            throw new InvalidOperationException("Кеш ранее был добавлен.");
        }

        if (!serviceCollection.Any(sd => sd.ServiceType == typeof(IFusionCache)))
        {
            throw new InvalidOperationException("Кеш не найден в DI.");
        }

        IsCacheAdded = true;

        return this;
    }

    public IBotPlatformBuilder Build()
    {
        if (!IsCacheAdded)
        {
            throw new InvalidOperationException("Отсутствует кеш.");
        }

        serviceCollection.AddSingleton<IDistributedLockFactory, DistributedLockFactory>();

        return botPlatformBuilder;
    }
}