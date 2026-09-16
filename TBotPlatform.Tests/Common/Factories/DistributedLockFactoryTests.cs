using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Common.Factories;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform.Tests.Common.Factories;

[TestFixture]
public class DistributedLockFactoryTests
{
    private ServiceProvider _provider = null!;
    private DistributedLockFactory _testClass = null!;

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();
        services.AddFusionCache();
        _provider = services.BuildServiceProvider();
        _testClass = new DistributedLockFactory(_provider.GetRequiredService<IFusionCache>());
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_provider is not null)
        {
            await _provider.DisposeAsync();
        }
    }

    [Test]
    public async Task CanCallAcquireLock()
    {
        var key = "TestValue1749272468";
        var timeOut = TimeSpan.FromSeconds(5);

        var result = await _testClass.AcquireLock(key, timeOut, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        var locked = await _testClass.IsLocked(key, CancellationToken.None);
        Assert.That(locked.IsSuccess && locked.Value, Is.True);

        await result.DisposeAsync();
        var after = await _testClass.IsLocked(key, CancellationToken.None);
        Assert.That(after.Value, Is.False);
    }

    [Test]
    public async Task SecondAcquire_WaitsUntilTimeout_WhenLockHeld()
    {
        const string key = "held-lock";
        await using var first = await _testClass.AcquireLock(key, TimeSpan.FromSeconds(5), CancellationToken.None);

        Assert.ThrowsAsync<TimeoutException>(async () =>
            await _testClass.AcquireLock(key, TimeSpan.FromMilliseconds(200), CancellationToken.None));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void CannotCallAcquireLockWithInvalidKey(string value) => Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AcquireLock(value, TimeSpan.FromSeconds(219), CancellationToken.None));

    [TestCase("")]
    [TestCase("   ")]
    public void CannotCallIsLockedWithInvalidKey(string value) => Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.IsLocked(value, CancellationToken.None));
}
