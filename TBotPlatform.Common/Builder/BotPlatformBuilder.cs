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

    private bool IsNeedAddHostedService { get; set; }
    private Assembly FactoriesExecutingAssembly { get; set; }

    public IBotBuilder AddBot(TelegramSettings telegramSettings)
    {
        Bots.Add(telegramSettings.BotName);

        return new BotBuilder(serviceCollection, this, telegramSettings);
    }

    public IBotPlatformBuilder AddHostedService()
    {
        IsNeedAddHostedService = true;

        return this;
    }

    public IBotPlatformBuilder AddFactories(Assembly executingAssembly)
    {
        FactoriesExecutingAssembly = executingAssembly;

        return this;
    }

    public IServiceCollection Build()
    {
        if (IsNeedAddHostedService)
        {
            serviceCollection.AddHostedService<TelegramContextHostedService>();
        }

        if (FactoriesExecutingAssembly.IsNull())
        {
            throw new(nameof(FactoriesExecutingAssembly));
        }

        serviceCollection
            .AddFactories(FactoriesExecutingAssembly)
            .AddSingleton(new BotsDataCollection(Bots));

        return serviceCollection;
    }
}
