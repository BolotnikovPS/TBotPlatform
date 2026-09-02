using TBotPlatform.Contracts.Abstractions.Cache.AsyncDisposable;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IDistributedLockFactory
{
    Task<IDistributedLock> AcquireLock(string key, TimeSpan timeOut, CancellationToken cancellationToken);

    Task<IResult<bool>> IsLocked(string key, CancellationToken cancellationToken);
}