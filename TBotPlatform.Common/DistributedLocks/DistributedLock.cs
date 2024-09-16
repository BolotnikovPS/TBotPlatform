using TBotPlatform.Common.Contracts;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Cache;

namespace TBotPlatform.Common.DistributedLocks;

internal class DistributedLock(ICacheService cacheService, string key) : IDistributedLock
{
    public async Task<IDistributedLock> RetryUntilTrueAsync(TimeSpan waitingTimeOut, TimeSpan blockingTimeOut, CancellationToken cancellationToken)
    {
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
        await cacheService.RemoveValueAsync(key, CancellationToken.None);
    }

    private async Task<bool> TryGetLockAsync(TimeSpan blockingTimeOut, CancellationToken cancellationToken)
    {
        if (!await cacheService.KeyExistsAsync(key, cancellationToken))
        {
            return await cacheService.SetValueAsync(CreateCacheValue(blockingTimeOut), cancellationToken);
        }

        var data = await cacheService.GetValueAsync<DistributedLockContract>(key, cancellationToken);

        if (data.Value > DateTime.UtcNow)
        {
            return false;
        }

        return await cacheService.SetValueAsync(CreateCacheValue(blockingTimeOut), cancellationToken);
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