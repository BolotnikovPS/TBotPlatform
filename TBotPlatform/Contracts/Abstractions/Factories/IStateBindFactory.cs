using TBotPlatform.Contracts.Bots;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateBindFactory
{
    /// <summary>
    /// Gets the bound state.
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">Chat id</param>
    /// <param name="cancellationToken"></param>
    Task<IResult<StateHistory>> GetBindStateOrNull(string botName, long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Binds the state.
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">Chat id</param>
    /// <param name="state">The state</param>
    /// <param name="cancellationToken"></param>
    Task<IResult> BindState(string botName, long chatId, StateHistory state, CancellationToken cancellationToken);

    /// <summary>
    /// Unbinds the state for the user.
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">Chat id</param>
    /// <param name="cancellationToken"></param>
    Task<IResult> UnBindState(string botName, long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether a state is bound.
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">Chat id</param>
    /// <param name="cancellationToken"></param>
    Task<IResult<bool>> HasBindState(string botName, long chatId, CancellationToken cancellationToken);
}