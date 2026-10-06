#nullable enable
using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Common.BackgroundServices;
using TBotPlatform.Common.Builder;
using TBotPlatform.Common.Factories;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Abstractions.Builder;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Abstractions.Queues;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Tests.Common.Builder;

/// <summary>
/// Tests the platform builder: adding bots, cache and factories, their build-time
/// validations, and the set of registered services.
/// </summary>
[TestFixture]
public class BotPlatformBuilderTests
{
    private ServiceCollection _services = null!;
    private BotPlatformBuilder _builder = null!;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _builder = new BotPlatformBuilder(_services);
    }

    [Test]
    public void AddBot_WhenBotNameAlreadyAdded_ThrowsInvalidOperationException()
    {
        _builder.AddBot(CreateSetting("TestBot"));

        var exception = Assert.Throws<InvalidOperationException>(() => _builder.AddBot(CreateSetting("testbot")));

        Assert.That(exception!.Message, Is.EqualTo("Бот ранее был добавлен."));
    }

    [Test]
    public void AddBot_WithTwoDifferentBots_ReturnsBotBuilderForBoth()
    {
        var first = _builder.AddBot(CreateSetting("first-bot"));
        var second = _builder.AddBot(CreateSetting("second-bot"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.InstanceOf<BotBuilder>());
            Assert.That(second, Is.InstanceOf<BotBuilder>());
            Assert.That(second, Is.Not.SameAs(first));
        }
    }

    [Test]
    public void AddCache_WhenCacheAlreadyAdded_ThrowsInvalidOperationException()
    {
        _builder.AddCache();

        var exception = Assert.Throws<InvalidOperationException>(() => _builder.AddCache());

        Assert.That(exception!.Message, Is.EqualTo("Кеш ранее был добавлен."));
    }

    [Test]
    public void AddCache_WithNewCache_ReturnsCacheBuilder()
    {
        var result = _builder.AddCache();

        Assert.That(result, Is.InstanceOf<CacheBuilder>());
    }

    [Test]
    public void AddFactories_WhenFactoriesAlreadyAdded_ThrowsInvalidOperationException()
    {
        _builder.AddFactories(GetType().Assembly);

        var exception = Assert.Throws<InvalidOperationException>(() => _builder.AddFactories(GetType().Assembly));

        Assert.That(exception!.Message, Is.EqualTo("Фабрики для работы ботов ранее были добавлены."));
    }

    [Test]
    public void AddFactories_WithAssembly_ReturnsSameBuilder()
    {
        var result = _builder.AddFactories(GetType().Assembly);

        Assert.That(result, Is.SameAs(_builder));
    }

    [Test]
    public void AddHostedService_ReturnsSameBuilder()
    {
        var result = _builder.AddHostedService();

        Assert.That(result, Is.SameAs(_builder));
    }

    [Test]
    public void Build_WhenBotListIsEmpty_ThrowsInvalidOperationException()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => _builder.Build());

        Assert.That(exception!.Message, Is.EqualTo("Список ботов пустой."));
    }

    [Test]
    public void Build_WhenCacheWasNotAdded_ThrowsInvalidOperationException()
    {
        _builder.AddBot(CreateSetting());

        var exception = Assert.Throws<InvalidOperationException>(() => _builder.Build());

        Assert.That(exception!.Message, Is.EqualTo("Кеш не был добавлен."));
    }

    [Test]
    public void Build_WhenFactoriesWereNotAdded_ThrowsInvalidOperationException()
    {
        AddBotWithMemoryCache();

        var exception = Assert.Throws<InvalidOperationException>(() => _builder.Build());

        Assert.That(exception!.Message, Is.EqualTo("Отсутствуют фабрики для работы ботов."));
    }

    [Test]
    public void Build_WhenAllDependenciesRegistered_ReturnsServiceCollection()
    {
        PreparePlatform();

        var result = _builder.Build();

        Assert.That(result, Is.SameAs(_services));
    }

    [Test]
    public void Build_WhenAllDependenciesRegistered_RegistersPlatformServices()
    {
        PreparePlatform();

        _builder.Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(GetDescriptor(typeof(IDelayQueue))?.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
            Assert.That(GetDescriptor(typeof(ITelegramUpdateProcessor))?.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
            Assert.That(GetDescriptor(typeof(IStateFactory))?.Lifetime, Is.EqualTo(ServiceLifetime.Scoped));
            Assert.That(GetDescriptor(typeof(IStateBindFactory))?.Lifetime, Is.EqualTo(ServiceLifetime.Scoped));
            Assert.That(GetDescriptor(typeof(IStateContextFactory))?.Lifetime, Is.EqualTo(ServiceLifetime.Scoped));
            Assert.That(GetDescriptor(typeof(IMenuButtonFactory))?.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
            Assert.That(GetDescriptor(typeof(IDistributedLockFactory))?.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
        }
    }

    [Test]
    public void Build_WhenHostedServiceWasNotRequested_RegistersOnlyDelayHostedService()
    {
        PreparePlatform();

        _builder.Build();

        var hostedServices = GetHostedServiceDescriptors();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hostedServices, Has.Count.EqualTo(1));
            Assert.That(hostedServices[0].ImplementationType, Is.EqualTo(typeof(TelegramDelayHostedService)));
        }
    }

    [Test]
    public void Build_WhenHostedServiceWasRequested_RegistersContextHostedService()
    {
        PreparePlatform(useHostedService: true);

        _builder.Build();

        var hostedServices = GetHostedServiceDescriptors();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hostedServices, Has.Count.EqualTo(2));
            Assert.That(
                hostedServices.Any(z => z.ImplementationFactory is not null),
                Is.True,
                "сервис контекста telegram регистрируется через фабрику");
        }
    }

    [Test]
    public void Build_WhenCalled_RegistersStateFactoryDependingOnFusionCache()
    {
        PreparePlatform();

        _builder.Build();

        Assert.That(GetDescriptor(typeof(StateFactory))?.Lifetime, Is.EqualTo(ServiceLifetime.Scoped));
    }

    private void PreparePlatform(bool useHostedService = false)
    {
        AddBotWithMemoryCache();

        if (useHostedService)
        {
            _builder.AddHostedService();
        }

        _builder.AddFactories(GetType().Assembly);
    }

    private void AddBotWithMemoryCache()
    {
        _builder.AddBot(CreateSetting());
        _builder.AddCache().AddMemoryFusionCache().Build();
    }

    private ServiceDescriptor? GetDescriptor(Type serviceType)
        => _services.SingleOrDefault(z => z.ServiceType == serviceType);

    /// <summary>
    /// The Microsoft.Extensions.Hosting.Abstractions package is not referenced by the test project directly,
    /// so the service type is identified by its full name.
    /// </summary>
    private List<ServiceDescriptor> GetHostedServiceDescriptors()
        => _services
           .Where(z => z.ServiceType.FullName == "Microsoft.Extensions.Hosting.IHostedService")
           .ToList();

    private static TBotSetting CreateSetting(string botName = "test-bot") => new()
    {
        BotName = botName,
        Token = "123:token",
    };
}
