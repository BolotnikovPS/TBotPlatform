using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Contracts.Abstractions;

public interface IMenuButton
{
    /// <summary>
    /// Gets the list of buttons for the state
    /// </summary>
    /// <param name="user">The user</param>
    Task<IResult<MainButtonMassiveList>> GetMainButtons<T>(T user)
        where T : UserBase;
}