using Moq;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Cache;

namespace TBotPlatform.Tests.Common.Factories;

[TestFixture]
public class DistributedLockFactoryTests
{
    private DistributedLockFactory _testClass;
    private Mock<ICacheService> _cacheService;

    [SetUp]
    public void SetUp()
    {
        _cacheService = new Mock<ICacheService>();
        _testClass = new DistributedLockFactory(_cacheService.Object);
    }

    [Test]
    public async Task CanCallAcquireLock()
    {
        // Arrange
        var key = "TestValue1749272468";
        var timeOut = TimeSpan.FromSeconds(79);
        var cancellationToken = CancellationToken.None;
        _cacheService.Setup(c => c.KeyExists(It.Is<string>(k => k == "Locker_" + key))).ReturnsAsync(false);
        _cacheService.Setup(c => c.SetValue(It.IsAny<IKeyInCache>())).ReturnsAsync(true);

        // Act
        var result = await _testClass.AcquireLock(key, timeOut, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        await result.DisposeAsync();
        _cacheService.Verify(c => c.RemoveValue("Locker_" + key), Times.Once);
    }

    [TestCase("")]
    [TestCase("   ")]
    public void CannotCallAcquireLockWithInvalidKey(string value) => Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AcquireLock(value, TimeSpan.FromSeconds(219), CancellationToken.None));

    [Test]
    public async Task CanCallIsLocked_WhenKeyExists_ReturnsTrue()
    {
        var key = "TestValue1948270513";
        _cacheService.Setup(c => c.KeyExists("Locker_" + key)).ReturnsAsync(true);

        var result = await _testClass.IsLocked(key, CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CanCallIsLocked_WhenKeyDoesNotExist_ReturnsFalse()
    {
        var key = "MissingKey";
        _cacheService.Setup(c => c.KeyExists("Locker_" + key)).ReturnsAsync(false);

        var result = await _testClass.IsLocked(key, CancellationToken.None);

        Assert.That(result, Is.False);
    }

    [TestCase("")]
    [TestCase("   ")]
    public void CannotCallIsLockedWithInvalidKey(string value) => Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.IsLocked(value, CancellationToken.None));
}
