using TBotPlatform.Common.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Factories;

internal class DistributedLockFactory(ICacheService cacheService) : IDistributedLockFactory
{
    public Task<IDistributedLock> AcquireLock(string key, TimeSpan timeOut, CancellationToken cancellationToken)
        => key.IsNull()
            ? throw new ArgumentNullException(nameof(key))
            : new DistributedLock(cacheService, CreateKey(key)).RetryUntilTrue(timeOut, timeOut, cancellationToken);

    public Task<bool> IsLocked(string key, CancellationToken cancellationToken)
        => key.IsNull()
            ? throw new ArgumentNullException(nameof(key))
            : cacheService.KeyExists(CreateKey(key));

    private static string CreateKey(string baseKey)
        => $"Locker_{baseKey}";
}