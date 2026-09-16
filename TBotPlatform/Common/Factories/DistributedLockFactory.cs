using TBotPlatform.Common.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Cache.Lock;
using TBotPlatform.Extension;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform.Common.Factories;

internal class DistributedLockFactory(IFusionCache cacheService) : IDistributedLockFactory
{
    public Task<IDistributedLock> AcquireLock(string key, TimeSpan timeOut, CancellationToken cancellationToken)
        => key.IsNull()
            ? throw new ArgumentNullException(nameof(key))
            : new DistributedLock(cacheService, CreateKey(key)).RetryUntilTrue(timeOut, timeOut, cancellationToken);

    public async Task<IResult<bool>> IsLocked(string key, CancellationToken cancellationToken)
    {
        if (key.IsNull())
        {
            throw new ArgumentNullException(nameof(key));
        }

        var stored = await cacheService.TryGetAsync<DistributedLockContract>(CreateKey(key), token: cancellationToken);
        var locked = stored.HasValue && stored.Value.Value > DateTime.UtcNow;
        return ResultT<bool>.Success(locked);
    }

    private static string CreateKey(string baseKey)
        => $"Locker_{baseKey}";
}