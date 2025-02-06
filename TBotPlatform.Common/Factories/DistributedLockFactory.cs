using TBotPlatform.Common.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;

namespace TBotPlatform.Common.Factories;

internal class DistributedLockFactory(ICacheService cacheService) : IDistributedLockFactory
{
    public Task<IDistributedLock> AcquireLock(string key, TimeSpan timeOut, CancellationToken cancellationToken)
        => new DistributedLock(cacheService, CreateKey(key)).RetryUntilTrue(timeOut, timeOut, cancellationToken);

    public Task<bool> IsLocked(string key, CancellationToken cancellationToken)
        => cacheService.KeyExists(CreateKey(key));

    private static string CreateKey(string baseKey)
        => $"Locker_{baseKey}";
}