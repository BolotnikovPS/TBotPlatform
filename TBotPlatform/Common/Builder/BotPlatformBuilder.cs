using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using TBotPlatform.Common.BackgroundServices;
using TBotPlatform.Common.Dependencies;
using TBotPlatform.Contracts.Abstractions.Builder;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Builder;

internal class BotPlatformBuilder(IServiceCollection serviceCollection) : IBotPlatformBuilder
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
            .AddFactories(FactoriesExecutingAssembly)
            .AddSingleton(new BotsDataCollection(Bots));

        return serviceCollection;
    }
}
