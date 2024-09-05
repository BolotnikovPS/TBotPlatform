using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Contracts.Abstractions.State;

public interface IStateBind
{
    /// <summary>
    /// Добавляет бинд для состояния
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="state">Состояние</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task BindStateAsync(long chatId, StateHistory state, CancellationToken cancellationToken);

    /// <summary>
    /// Снимает бинд состояний для пользователя
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnBindStateAsync(long chatId, CancellationToken cancellationToken);
}