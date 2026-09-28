#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TBotPlatform.Common.Builder;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Builder;
using TBotPlatform.Contracts.Abstractions.Factories;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform.Tests.Common.Builder;

/// <summary>
/// Проверяет сборщик кеша: выбор Redis / memory / пользовательского кеша, запрет повторной
/// регистрации кеша и регистрацию распределённой блокировки при сборке.
/// </summary>
[TestFixture]
public class CacheBuilderTests
{
    private IServiceCollection _services = null!;
    private Mock<IBotPlatformBuilder> _platformBuilder = null!;
    private CacheBuilder _builder = null!;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _platformBuilder = new Mock<IBotPlatformBuilder>();
        _builder = new CacheBuilder(_services, _platformBuilder.Object);
    }

    [TestCase("")]
    [TestCase("   ")]
    public void AddRedisFusionCache_WhenConnectionStringIsEmpty_ThrowsInvalidOperationException(string connectionString)
    {
        var exception = Assert.Throws<InvalidOperationException>(() => _builder.AddRedisFusionCache(connectionString));

        Assert.That(exception!.Message, Is.EqualTo("Строка подключения пуста."));
    }

    [Test]
    public void AddRedisFusionCache_WithValidConnectionString_ReturnsRedisBuilder()
    {
        var result = _builder.AddRedisFusionCache("localhost:6379");

        Assert.That(result, Is.InstanceOf<RedisBuilder>());
    }

    [Test]
    public void AddRedisFusionCache_WhenCacheAlreadyAdded_ThrowsInvalidOperationException()
    {
        _builder.AddRedisFusionCache("localhost:6379");

        var exception = Assert.Throws<InvalidOperationException>(() => _builder.AddRedisFusionCache("localhost:6379"));

        Assert.That(exception!.Message, Is.EqualTo("Кеш ранее был добавлен."));
    }

    [Test]
    public void AddMemoryFusionCache_WhenCacheAlreadyAdded_ThrowsInvalidOperationException()
    {
        _builder.AddRedisFusionCache("localhost:6379");

        var exception = Assert.Throws<InvalidOperationException>(() => _builder.AddMemoryFusionCache());

        Assert.That(exception!.Message, Is.EqualTo("Кеш ранее был добавлен."));
    }

    [Test]
    public void AddMemoryFusionCache_WithNewCache_RegistersFusionCacheAndReturnsSelf()
    {
        var result = _builder.AddMemoryFusionCache();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.SameAs(_builder));
            Assert.That(_services.Any(z => z.ServiceType == typeof(IFusionCache)), Is.True);
        }
    }

    [Test]
    public void CheckCustomFusionCache_WhenCacheIsNotRegisteredInDi_ThrowsInvalidOperationException()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => _builder.CheckCustomFusionCache());

        Assert.That(exception!.Message, Is.EqualTo("Кеш не найден в DI."));
    }

    [Test]
    public void CheckCustomFusionCache_WhenCacheIsRegisteredInDi_ReturnsSelf()
    {
        _services.AddFusionCache();

        var result = _builder.CheckCustomFusionCache();

        Assert.That(result, Is.SameAs(_builder));
    }

    [Test]
    public void CheckCustomFusionCache_WhenCacheAlreadyAdded_ThrowsInvalidOperationException()
    {
        _builder.AddMemoryFusionCache();

        var exception = Assert.Throws<InvalidOperationException>(() => _builder.CheckCustomFusionCache());

        Assert.That(exception!.Message, Is.EqualTo("Кеш ранее был добавлен."));
    }

    [Test]
    public void Build_WhenCacheWasNotAdded_ThrowsInvalidOperationException()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => _builder.Build());

        Assert.That(exception!.Message, Is.EqualTo("Отсутствует кеш."));
    }

    [Test]
    public void Build_WhenMemoryCacheAdded_ReturnsPlatformBuilder()
    {
        _builder.AddMemoryFusionCache();

        var result = _builder.Build();

        Assert.That(result, Is.SameAs(_platformBuilder.Object));
    }

    [Test]
    public void Build_WhenCacheWasAdded_RegistersDistributedLockFactoryAsSingleton()
    {
        _builder.AddMemoryFusionCache();

        _builder.Build();

        var descriptor = _services.SingleOrDefault(z => z.ServiceType == typeof(IDistributedLockFactory));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(descriptor, Is.Not.Null);
            Assert.That(descriptor!.ImplementationType, Is.EqualTo(typeof(DistributedLockFactory)));
            Assert.That(descriptor.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
        }
    }

    [Test]
    public void Build_WhenCustomCacheChecked_RegistersDistributedLockFactory()
    {
        _services.AddFusionCache();
        _builder.CheckCustomFusionCache();

        _builder.Build();

        Assert.That(_services.Any(z => z.ServiceType == typeof(IDistributedLockFactory)), Is.True);
    }
}
