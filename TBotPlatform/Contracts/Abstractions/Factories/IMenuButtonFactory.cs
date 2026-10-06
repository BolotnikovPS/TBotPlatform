using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IMenuButtonFactory
{
    /// <summary>
    /// Gets the main menu buttons.
    /// </summary>
    /// <param name="user">User to interact with</param>
    /// <param name="stateHistory">State being opened</param>
    Task<IResult<MainButtonMassiveList>> GetMainButtons<T>(T user, StateHistory stateHistory)
        where T : UserBase;

    /// <summary>
    /// Gets the main menu buttons by menu type.
    /// </summary>
    /// <param name="user">User to interact with</param>
    /// <param name="menuStateType">Menu being opened</param>
    Task<IResult<MainButtonMassiveList>> GetMainButtons<T>(T user, Type menuStateType)
        where T : UserBase;

    /// <summary>
    /// Updates main menu buttons by invoked state.
    /// </summary>
    /// <param name="user">User to interact with</param>
    /// <param name="stateContext">State context</param>
    /// <param name="stateHistory">State being opened</param>
    /// <param name="cancellationToken"></param>
    Task<IResult> UpdateMainButtonsByState<T>(T user, IStateContextMinimal stateContext, StateHistory stateHistory, CancellationToken cancellationToken)
        where T : UserBase;

    /// <summary>
    /// Updates main menu buttons by menu type.
    /// </summary>
    /// <param name="user">User to interact with</param>
    /// <param name="stateContext">State context</param>
    /// <param name="menuStateType">Menu being opened</param>
    /// <param name="cancellationToken"></param>
    Task<IResult> UpdateMainButtonsByState<T>(T user, IStateContextMinimal stateContext, Type menuStateType, CancellationToken cancellationToken)
        where T : UserBase;
}