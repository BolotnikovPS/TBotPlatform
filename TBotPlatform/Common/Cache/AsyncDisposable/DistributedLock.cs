using TBotPlatform.Contracts.Abstractions.Cache.AsyncDisposable;
using TBotPlatform.Contracts.Cache.Lock;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform.Common.Cache.AsyncDisposable;

internal class DistributedLock(IFusionCache cacheService, string key) : IDistributedLock
{
    public async Task<IDistributedLock> RetryUntilTrue(TimeSpan waitingTimeOut, TimeSpan blockingTimeOut, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var i = 0;
        var initialTime = DateTime.UtcNow;

        while (DateTime.UtcNow - initialTime < waitingTimeOut)
        {
            i++;
            
            if ((await TryGetLock(blockingTimeOut, cancellationToken)).IsSuccess)
            {
                return this;
            }

            Sleep(i);
        }

        throw new TimeoutException($"Превышено время ожидания {waitingTimeOut} для ключа {key}");
    }

    public async ValueTask DisposeAsync() => await cacheService.RemoveValue(key);

    private async Task<IResult> TryGetLock(TimeSpan blockingTimeOut, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!(await cacheService.KeyExists(key)).Value)
        {
            return await cacheService.SetValue(CreateCacheValue(blockingTimeOut));
        }

        var data = await cacheService.GetValue<DistributedLockContract>(key);

        if (!data.IsSuccess || data.Value.Value > DateTime.UtcNow)
        {
            return Result.Failure(ErrorResult.None());
        }

        return await cacheService.SetValue(CreateCacheValue(blockingTimeOut));
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