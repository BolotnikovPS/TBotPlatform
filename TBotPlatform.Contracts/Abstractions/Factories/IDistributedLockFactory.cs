using TBotPlatform.Contracts.Abstractions.Cache.AsyncDisposable;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IDistributedLockFactory
{
    Task<IDistributedLock> AcquireLock(string key, TimeSpan timeOut, CancellationToken cancellationToken);

    Task<bool> IsLocked(string key, CancellationToken cancellationToken);
}