using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Common.BackgroundServices;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Builder;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Builder;

internal partial class BotPlatformBuilder(IServiceCollection serviceCollection) : IBotPlatformBuilder
{
    private readonly List<string> Bots = [];

    private bool IsCacheAdded { get; set; }
    private bool IsNeedAddHostedService { get; set; }
    private Assembly FactoriesExecutingAssembly { get; set; }

    public IBotBuilder AddBot(TelegramSettings telegramSettings)
    {
        if (Bots.Any(z => z.Equals(telegramSettings.BotName, StringComparison.CurrentCultureIgnoreCase)))
        {
            throw new InvalidOperationException("Бот ранее был добавлен.");
        }

        Bots.Add(telegramSettings.BotName);

        return new BotBuilder(serviceCollection, this, telegramSettings);
    }

    public ICacheBuilder AddCache()
    {
        if (IsCacheAdded)
        {
            throw new InvalidOperationException("Кеш ранее был добавлен.");
        }

        IsCacheAdded = true;

        return new CacheBuilder(serviceCollection, this);
    }

    public IBotPlatformBuilder AddHostedService()
    {
        IsNeedAddHostedService = true;

        return this;
    }

    public IBotPlatformBuilder AddFactories(Assembly executingAssembly)
    {
        if (FactoriesExecutingAssembly.IsNotNull())
        {
            throw new InvalidOperationException("Фабрики для работы ботов ранее были добавлены.");
        }

        FactoriesExecutingAssembly = executingAssembly;

        return this;
    }

    public IServiceCollection Build()
    {
        if (Bots.Count == 0)
        {
            throw new InvalidOperationException("Список ботов пустой.");
        }

        if (!IsCacheAdded)
        {
            throw new InvalidOperationException("Кеш не был добавлен.");
        }

        if (FactoriesExecutingAssembly.IsNull())
        {
            throw new InvalidOperationException("Отсутствуют фабрики для работы ботов.");
        }

        if (IsNeedAddHostedService)
        {
            serviceCollection.AddHostedService<TelegramContextHostedService>();
        }

        serviceCollection
           .AddScoped(s =>
           {
               var cache = s.GetRequiredService<ICacheService>();

               return new StateFactory(cache, s, FactoriesExecutingAssembly);
           })
           .AddScoped<IStateFactory>(src => src.GetRequiredService<StateFactory>())
           .AddScoped<IStateBindFactory>(src => src.GetRequiredService<StateFactory>())
           .AddScoped<IStateContextFactory, StateContextFactory>()
           .AddSingleton<IMenuButtonFactory, MenuButtonFactory>()
           .AddSingleton(new BotsDataCollection(Bots));

        return serviceCollection;
    }
}
