using Moq;
using TBotPlatform.Common.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Cache.Lock;

namespace TBotPlatform.Tests.Common.Cache.AsyncDisposable;

[TestFixture]
public class DistributedLockTests
{
    private Mock<ICacheService> _cacheService = null!;
    private DistributedLock _lock = null!;
    private const string TestKey = "test-key";

    [SetUp]
    public void SetUp()
    {
        _cacheService = new Mock<ICacheService>();
        _lock = new DistributedLock(_cacheService.Object, TestKey);
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_lock != null)
        {
            await _lock.DisposeAsync();
        }
    }

    [Test]
    public async Task TryGetLock_WhenKeyDoesNotExist_AcquiresLock()
    {
        _cacheService.Setup(c => c.KeyExists(TestKey)).ReturnsAsync(false);
        _cacheService.Setup(c => c.SetValue(It.IsAny<IKeyInCache>())).ReturnsAsync(true);

        var result = await InvokeTryGetLock(TimeSpan.FromSeconds(10));

        Assert.That(result, Is.True);
        _cacheService.Verify(c => c.SetValue(It.IsAny<IKeyInCache>()), Times.Once);
    }

    [Test]
    public async Task TryGetLock_WhenKeyExistsAndLockExpired_AcquiresLock()
    {
        var expiredLock = new DistributedLockContract
        {
            Key = TestKey,
            Value = DateTime.UtcNow.AddMinutes(-1)
        };

        _cacheService.Setup(c => c.KeyExists(TestKey)).ReturnsAsync(true);
        _cacheService.Setup(c => c.GetValue<DistributedLockContract>(TestKey)).ReturnsAsync(expiredLock);
        _cacheService.Setup(c => c.SetValue(It.IsAny<IKeyInCache>())).ReturnsAsync(true);

        var result = await InvokeTryGetLock(TimeSpan.FromSeconds(10));

        Assert.That(result, Is.True);
        _cacheService.Verify(c => c.SetValue(It.IsAny<IKeyInCache>()), Times.Once);
    }

    [Test]
    public async Task TryGetLock_WhenKeyExistsAndLockNotExpired_ReturnsFalse()
    {
        var activeLock = new DistributedLockContract
        {
            Key = TestKey,
            Value = DateTime.UtcNow.AddMinutes(5)
        };

        _cacheService.Setup(c => c.KeyExists(TestKey)).ReturnsAsync(true);
        _cacheService.Setup(c => c.GetValue<DistributedLockContract>(TestKey)).ReturnsAsync(activeLock);

        var result = await InvokeTryGetLock(TimeSpan.FromSeconds(10));

        Assert.That(result, Is.False);
        _cacheService.Verify(c => c.SetValue(It.IsAny<IKeyInCache>()), Times.Never);
    }

    [Test]
    public async Task TryGetLock_WhenKeyExistsButDataIsNull_ReturnsFalse()
    {
        _cacheService.Setup(c => c.KeyExists(TestKey)).ReturnsAsync(true);
        _cacheService.Setup(c => c.GetValue<DistributedLockContract>(TestKey)).ReturnsAsync((DistributedLockContract?)default);

        var result = await InvokeTryGetLock(TimeSpan.FromSeconds(10));

        Assert.That(result, Is.False);
        _cacheService.Verify(c => c.SetValue(It.IsAny<IKeyInCache>()), Times.Never);
    }

    [Test]
    public async Task DisposeAsync_RemovesKeyFromCache()
    {
        await _lock.DisposeAsync();

        _cacheService.Verify(c => c.RemoveValue(TestKey), Times.Once);
    }

    [Test]
    public async Task RetryUntilTrue_WhenLockAcquired_ReturnsLock()
    {
        _cacheService.Setup(c => c.KeyExists(TestKey)).ReturnsAsync(false);
        _cacheService.Setup(c => c.SetValue(It.IsAny<IKeyInCache>())).ReturnsAsync(true);

        var result = await _lock.RetryUntilTrue(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), CancellationToken.None);

        Assert.That(result, Is.SameAs(_lock));
    }

    [Test]
    public void RetryUntilTrue_WhenTimeout_ThrowsTimeoutException()
    {
        _cacheService.Setup(c => c.KeyExists(TestKey)).ReturnsAsync(true);
        _cacheService.Setup(c => c.GetValue<DistributedLockContract>(TestKey))
            .ReturnsAsync(new DistributedLockContract { Key = TestKey, Value = DateTime.UtcNow.AddMinutes(5) });

        Assert.ThrowsAsync<TimeoutException>(async () =>
            await _lock.RetryUntilTrue(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(10), CancellationToken.None));
    }

    [Test]
    public void RetryUntilTrue_WhenCancelled_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _lock.RetryUntilTrue(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), cts.Token));
    }

    private async Task<bool> InvokeTryGetLock(TimeSpan blockingTimeOut)
    {
        var method = typeof(DistributedLock).GetMethod("TryGetLock",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ?? throw new InvalidOperationException("TryGetLock method not found");

        var task = (Task<bool>)method.Invoke(_lock, [blockingTimeOut, CancellationToken.None])!;
        return await task;
    }
}
