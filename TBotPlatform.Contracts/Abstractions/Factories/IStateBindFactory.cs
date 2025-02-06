using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateBindFactory
{
    /// <summary>
    /// Получает зафиксированное состояние или null
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetBindStateOrNull(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Фиксирует состояние
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="state">Состояние</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task BindState(long chatId, StateHistory state, CancellationToken cancellationToken);

    /// <summary>
    /// Снимает фиксацию состояния для пользователя
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnBindState(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Проверяет наличие зафиксированного состояния
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HasBindState(long chatId, CancellationToken cancellationToken);
}