using Moq;
using TBotPlatform.Common.Factories;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform.Tests.Common.Factories;

[TestFixture]
public class DistributedLockFactoryTests
{
    private DistributedLockFactory _testClass;
    private Mock<IFusionCache> _cacheService;

    [SetUp]
    public void SetUp()
    {
        _cacheService = new Mock<IFusionCache>();
        _testClass = new DistributedLockFactory(_cacheService.Object);
    }

    [Test]
    public async Task CanCallAcquireLock()
    {
        // Arrange
        var key = "TestValue1749272468";
        var timeOut = TimeSpan.FromSeconds(79);
        var cancellationToken = CancellationToken.None;
        _cacheService.Setup(c => c.TryGetAsync<string>(It.Is<string>(k => k == "Locker_" + key), null, cancellationToken)).ReturnsAsync(MaybeValue<string>.None);
        _cacheService.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<string>(), null, null, cancellationToken)).Returns(ValueTask.CompletedTask);

        // Act
        var result = await _testClass.AcquireLock(key, timeOut, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        await result.DisposeAsync();
        _cacheService.Verify(c => c.RemoveAsync("Locker_" + key, null, cancellationToken), Times.Once);
    }

    [TestCase("")]
    [TestCase("   ")]
    public void CannotCallAcquireLockWithInvalidKey(string value) => Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AcquireLock(value, TimeSpan.FromSeconds(219), CancellationToken.None));

    [TestCase("")]
    [TestCase("   ")]
    public void CannotCallIsLockedWithInvalidKey(string value) => Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.IsLocked(value, CancellationToken.None));
}
