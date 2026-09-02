using TBotPlatform.Common.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Extension;
using TBotPlatform.Results.Abstractions;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform.Common.Factories;

internal class DistributedLockFactory(IFusionCache cacheService) : IDistributedLockFactory
{
    public Task<IDistributedLock> AcquireLock(string key, TimeSpan timeOut, CancellationToken cancellationToken)
        => key.IsNull()
            ? throw new ArgumentNullException(nameof(key))
            : new DistributedLock(cacheService, CreateKey(key)).RetryUntilTrue(timeOut, timeOut, cancellationToken);

    public Task<IResult<bool>> IsLocked(string key, CancellationToken cancellationToken)
        => key.IsNull()
            ? throw new ArgumentNullException(nameof(key))
            : cacheService.KeyExists(CreateKey(key));

    private static string CreateKey(string baseKey)
        => $"Locker_{baseKey}";
}