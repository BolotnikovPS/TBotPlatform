using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.State;

public abstract class BaseStateHandler<T> : IState<T>
    where T : UserBase
{
    public abstract Task HandleAsync(IStateContext context, T user, CancellationToken cancellationToken);

    public virtual Task HandleCompleteAsync(IStateContext context, T user, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public virtual Task HandleErrorAsync(IStateContext context, T user, Exception exception, CancellationToken cancellationToken)
        => Task.CompletedTask;
}