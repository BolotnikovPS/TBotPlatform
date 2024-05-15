using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateFactory<T>
where T : UserBase
{
    /// <summary>
    /// Получает состояние по его названию
    /// </summary>
    /// <param name="nameOfState"></param>
    /// <returns></returns>
    StateHistory<T> GetStateByNameOrDefault(string nameOfState = "");

    /// <summary>
    /// Получает состояние по типу кнопки
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="buttonTypeValue"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory<T>> GetStateByButtonsTypeOrDefaultAsync(long chatId, string buttonTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние по типу комманды
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="commandTypeValue"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory<T>> GetStateByCommandsTypeOrDefaultAsync(long chatId, string commandTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние по типу текста
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="textTypeValue"></param>
    /// <returns></returns>
    StateHistory<T> GetStateByTextsTypeOrDefault(long chatId, string textTypeValue);

    /// <summary>
    /// Получает первоначальное состояние
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory<T>> GetStateMainAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает предыдущие или первоначальное состояние, у которого есть меню
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory<T>> GetStatePreviousOrMainAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает последнее меню, которое было в истории состояний
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IMenuButton<T>> GetLastMenuAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние для заблокированных пользователей
    /// </summary>
    /// <returns></returns>
    StateHistory<T> GetLockState();
}