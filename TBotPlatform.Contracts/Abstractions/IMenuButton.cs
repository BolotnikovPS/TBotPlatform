using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;

namespace TBotPlatform.Contracts.Abstractions;

public interface IMenuButton<in T>
    where T : UserBase
{
    /// <summary>
    /// Получает список кнопок для состояния
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <returns></returns>
    ButtonsRuleMassiveList GetMarkUp(T user);
}