using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateFactory<T>
where T : UserBase
{
    /// <summary>
    /// Получает состояние по его названию
    /// </summary>
    /// <param name="nameOfState">Название состояния</param>
    /// <returns></returns>
    StateHistory<T> GetStateByNameOrDefault(string nameOfState = "");

    /// <summary>
    /// Получает состояние по типу кнопки
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="buttonTypeValue">Тип кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory<T>> GetStateByButtonsTypeOrDefaultAsync(long chatId, string buttonTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние по типу команды
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="commandTypeValue">Тип команды</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory<T>> GetStateByCommandsTypeOrDefaultAsync(long chatId, string commandTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние по типу текста
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="textTypeValue">Тип текста</param>
    /// <returns></returns>
    StateHistory<T> GetStateByTextsTypeOrDefault(long chatId, string textTypeValue);

    /// <summary>
    /// Получает первоначальное состояние
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory<T>> GetStateMainAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает предыдущие или первоначальное состояние, у которого есть меню
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory<T>> GetStatePreviousOrMainAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает последнее меню, которое было в истории состояний
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IMenuButton<T>> GetLastMenuAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние для заблокированных пользователей
    /// </summary>
    /// <returns></returns>
    StateHistory<T> GetLockState();

    /// <summary>
    /// Получает забиндинное состояние или null
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory<T>> GetBindStateOrNullAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет бинд для состояния
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="state">Состояние</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task BindStateAsync(long chatId, StateHistory<T> state, CancellationToken cancellationToken);

    /// <summary>
    /// Снимает бинд состояний для пользователя
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnBindStateAsync(long chatId, CancellationToken cancellationToken);
}