using TBotPlatform.Common.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;

namespace TBotPlatform.Common.Factories;

internal class DistributedLockFactory(ICacheService cacheService) : IDistributedLockFactory
{
    public Task<IDistributedLock> AcquireLockAsync(string key, TimeSpan timeOut, CancellationToken cancellationToken)
        => new DistributedLock(cacheService, CreateKey(key)).RetryUntilTrueAsync(timeOut, timeOut, cancellationToken);

    public Task<bool> IsLockedAsync(string key, CancellationToken cancellationToken)
        => cacheService.KeyExistsAsync(CreateKey(key), cancellationToken);

    public static string CreateKey(string baseKey)
        => $"Locker_{baseKey}";
}