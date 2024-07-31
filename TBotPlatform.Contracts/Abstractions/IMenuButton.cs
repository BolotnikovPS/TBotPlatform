using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;

namespace TBotPlatform.Contracts.Abstractions;

public interface IMenuButton
{
    /// <summary>
    /// Получает список кнопок для состояния
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <returns></returns>
    Task<ButtonsRuleMassiveList> GetMarkUpAsync<T>(T user)
        where T : UserBase;
}