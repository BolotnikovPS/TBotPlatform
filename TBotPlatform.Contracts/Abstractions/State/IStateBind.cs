using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Contracts.Abstractions.State;

public interface IStateBind
{
    /// <summary>
    /// Получает зафиксированное состояние или null
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetBindStateOrNullAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Фиксирует состояние
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="state">Состояние</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task BindStateAsync(long chatId, StateHistory state, CancellationToken cancellationToken);

    /// <summary>
    /// Снимает фиксацию состояния для пользователя
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnBindStateAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Проверяет наличие зафиксированного состояния
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HasBindStateAsync(long chatId, CancellationToken cancellationToken);
}