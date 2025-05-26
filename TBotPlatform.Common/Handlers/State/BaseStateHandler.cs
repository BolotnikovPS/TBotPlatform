using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Common.Handlers.State;

public abstract class BaseStateHandler<T> : IState<T>
    where T : UserBase
{
    public abstract Task Handle(IStateContext context, T user, CancellationToken cancellationToken);

    public virtual Task HandleComplete(IStateContext context, T user, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public virtual Task HandleError(IStateContext context, T user, Exception exception, CancellationToken cancellationToken)
        => Task.CompletedTask;
}