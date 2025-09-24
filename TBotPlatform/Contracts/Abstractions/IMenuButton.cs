using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Contracts.Abstractions;

public interface IMenuButton
{
    /// <summary>
    /// Получает список кнопок для состояния
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <returns></returns>
    Task<IResult<MainButtonMassiveList>> GetMainButtons<T>(T user)
        where T : UserBase;
}