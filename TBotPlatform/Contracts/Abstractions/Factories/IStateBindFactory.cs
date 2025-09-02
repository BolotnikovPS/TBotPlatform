using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateBindFactory
{
    /// <summary>
    /// Получает зафиксированное состояние или null
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetBindStateOrNull(string botName, long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Фиксирует состояние
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="state">Состояние</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task BindState(string botName, long chatId, StateHistory state, CancellationToken cancellationToken);

    /// <summary>
    /// Снимает фиксацию состояния для пользователя
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnBindState(string botName, long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Проверяет наличие зафиксированного состояния
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HasBindState(string botName, long chatId, CancellationToken cancellationToken);
}