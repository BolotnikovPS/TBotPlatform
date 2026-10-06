using TBotPlatform.Contracts.Bots;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateFactory : IStateBindFactory
{
    /// <summary>
    /// Checks if a state exists
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="nameOfState">State name</param>
    IResult<bool> HasState(string botName, string nameOfState);

    /// <summary>
    /// Checks if states exist
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="nameOfStates">State names</param>
    IResult<bool> HasStates(string botName, string[] nameOfStates);

    /// <summary>
    /// Gets a state by its name or /start
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="nameOfState">State name</param>
    IResult<StateHistory> GetStateByNameOrDefault(string botName, string nameOfState = "");

    /// <summary>
    /// Gets a state by button type or /start
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">Chat id</param>
    /// <param name="buttonTypeValue">Button type</param>
    /// <param name="cancellationToken"></param>
    Task<IResult<StateHistory>> GetStateByButtonsTypeOrDefault(string botName, long chatId, string buttonTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a state by command type or /start
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">Chat id</param>
    /// <param name="commandTypeValue">Command type</param>
    /// <param name="cancellationToken"></param>
    Task<IResult<StateHistory>> GetStateByCommandsTypeOrDefault(string botName, long chatId, string commandTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a state by text type or /start
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">Chat id</param>
    /// <param name="textTypeValue">Text type</param>
    IResult<StateHistory> GetStateByTextsTypeOrDefault(string botName, long chatId, string textTypeValue);

    /// <summary>
    /// Gets the initial state or /start
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">Chat id</param>
    /// <param name="cancellationToken"></param>
    Task<IResult<StateHistory>> GetStateMain(string botName, long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the previous or initial state that has a menu or /start
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">Chat id</param>
    /// <param name="cancellationToken"></param>
    Task<IResult<StateHistory>> GetStatePreviousOrMain(string botName, long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the last state with a menu from the state history
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">Chat id</param>
    /// <param name="cancellationToken"></param>
    Task<IResult<StateHistory>> GetLastStateWithMenu(string botName, long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a state for blocked users
    /// </summary>
    /// <param name="botName">Bot name</param>
    IResult<StateHistory> GetLockState(string botName);

    /// <summary>
    /// Gets a state for registration
    /// </summary>
    /// <param name="botName">Bot name</param>
    IResult<StateHistory> GetRegistrationState(string botName);
}