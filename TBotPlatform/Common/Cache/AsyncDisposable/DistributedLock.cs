using TBotPlatform.Contracts.Abstractions.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Cache.Lock;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform.Common.Cache.AsyncDisposable;

internal class DistributedLock(IFusionCache cacheService, string key) : IDistributedLock
{
    private readonly Guid _ownerId = Guid.NewGuid();

    public async Task<IDistributedLock> RetryUntilTrue(TimeSpan waitingTimeOut, TimeSpan blockingTimeOut, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var attempt = 0;
        var initialTime = DateTime.UtcNow;

        while (DateTime.UtcNow - initialTime < waitingTimeOut)
        {
            attempt++;

            if ((await TryGetLock(blockingTimeOut, cancellationToken)).IsSuccess)
            {
                return this;
            }

            await Task.Delay(GetBackoff(attempt), cancellationToken);
        }

        throw new TimeoutException($"Превышено время ожидания {waitingTimeOut} для ключа {key}");
    }

    public async ValueTask DisposeAsync()
    {
        var current = await cacheService.TryGetAsync<DistributedLockContract>(key);
        if (current.HasValue && current.Value.OwnerId == _ownerId)
        {
            await cacheService.RemoveAsync(key);
        }
    }

    private async Task<IResult> TryGetLock(TimeSpan blockingTimeOut, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var existing = await cacheService.TryGetAsync<DistributedLockContract>(key, token: cancellationToken);
        if (existing.HasValue && existing.Value.Value > DateTime.UtcNow && existing.Value.OwnerId != _ownerId)
        {
            return Result.Failure(ErrorResult.None());
        }

        var contract = new DistributedLockContract
        {
            Key = key,
            OwnerId = _ownerId,
            Value = DateTime.UtcNow.Add(blockingTimeOut),
        };

        var options = new FusionCacheEntryOptions
        {
            Duration = blockingTimeOut,
            IsFailSafeEnabled = false,
            AllowBackgroundDistributedCacheOperations = false,
        };

        await cacheService.SetAsync(key, contract, options, token: cancellationToken);

        var stored = await cacheService.TryGetAsync<DistributedLockContract>(key, token: cancellationToken);
        return stored.HasValue && stored.Value.OwnerId == _ownerId
            ? Result.Success()
            : Result.Failure(ErrorResult.None());
    }

    private static TimeSpan GetBackoff(int attempt)
    {
        var capped = Math.Min(attempt, 10);
        var maxMs = (int)Math.Pow(capped + 1, 2.0) + 1;
        var minMs = (int)Math.Pow(capped, 2.0);
        return TimeSpan.FromMilliseconds(Random.Shared.Next(Math.Max(minMs, 1), maxMs + 1));
    }
}
