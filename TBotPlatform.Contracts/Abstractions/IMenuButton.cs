using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Abstractions;

public interface IMenuButton
{
    /// <summary>
    /// Получает список кнопок для состояния
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <returns></returns>
    Task<MainButtonMassiveList> GetMainButtonsAsync<T>(T user)
        where T : UserBase;
}