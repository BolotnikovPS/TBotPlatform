using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Abstractions.State;

public interface IState<in T>
    where T : UserBase
{
    /// <summary>
    /// Starts the processing of the state logic
    /// </summary>
    /// <param name="user">The user</param>
    /// <param name="cancellationToken"></param>
    /// <param name="context">Context for processing the message</param>
    Task Handle(IStateContext context, T user, CancellationToken cancellationToken);

    /// <summary>
    /// Starts the processing of the state logic after HandleAsync
    /// </summary>
    /// <param name="user">The user</param>
    /// <param name="cancellationToken"></param>
    /// <param name="context">Context for processing the message</param>
    Task HandleComplete(IStateContext context, T user, CancellationToken cancellationToken);

    /// <summary>
    /// Starts the processing of the state logic in case of an error
    /// </summary>
    /// <param name="user">The user</param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="context">Context for processing the message</param>
    Task HandleError(IStateContext context, T user, Exception exception, CancellationToken cancellationToken);
}