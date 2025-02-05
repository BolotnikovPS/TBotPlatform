using TBotPlatform.Common.Contracts;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Abstractions.Cache.AsyncDisposable;

namespace TBotPlatform.Common.Cache.AsyncDisposable;

internal class DistributedLock(ICacheService cacheService, string key) : IDistributedLock
{
    public async Task<IDistributedLock> RetryUntilTrueAsync(TimeSpan waitingTimeOut, TimeSpan blockingTimeOut, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var i = 0;
        var initialTime = DateTime.UtcNow;

        while (DateTime.UtcNow - initialTime < waitingTimeOut)
        {
            i++;

            if (await TryGetLockAsync(blockingTimeOut, cancellationToken))
            {
                return this;
            }

            Sleep(i);
        }

        throw new TimeoutException($"Превышено время ожидания {waitingTimeOut} для ключа {key}");
    }

    public async ValueTask DisposeAsync()
    {
        await cacheService.RemoveValueAsync(key);
    }

    private async Task<bool> TryGetLockAsync(TimeSpan blockingTimeOut, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!await cacheService.KeyExistsAsync(key))
        {
            return await cacheService.SetValueAsync(CreateCacheValue(blockingTimeOut));
        }

        var data = await cacheService.GetValueAsync<DistributedLockContract>(key);

        if (data.Value > DateTime.UtcNow)
        {
            return false;
        }

        return await cacheService.SetValueAsync(CreateCacheValue(blockingTimeOut));
    }

    private DistributedLockContract CreateCacheValue(TimeSpan blockingTimeOut)
        => new()
        {
            Key = key,
            Value = DateTime.UtcNow.Add(blockingTimeOut),
        };

    private static void Sleep(int multiplier)
    {
        var random = new Random(Guid.NewGuid().GetHashCode());
        var timeout = random.Next((int)Math.Pow(multiplier, y: 2.0), (int)Math.Pow(multiplier + 1, y: 2.0) + 1);

        Thread.Sleep(timeout);
    }
}