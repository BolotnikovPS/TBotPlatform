namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IDistributedLockFactory
{
    Task<IDistributedLock> AcquireLockAsync(string key, TimeSpan timeOut, CancellationToken cancellationToken);

    Task<bool> IsLockedAsync(string key, CancellationToken cancellationToken);
}