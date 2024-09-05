using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateFactory : IStateBind
{
    /// <summary>
    /// Получает состояние по его названию
    /// </summary>
    /// <param name="nameOfState">Название состояния</param>
    /// <returns></returns>
    StateHistory GetStateByNameOrDefault(string nameOfState = "");

    /// <summary>
    /// Получает состояние по типу кнопки
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="buttonTypeValue">Тип кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetStateByButtonsTypeOrDefaultAsync(long chatId, string buttonTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние по типу команды
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="commandTypeValue">Тип команды</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetStateByCommandsTypeOrDefaultAsync(long chatId, string commandTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние по типу текста
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="textTypeValue">Тип текста</param>
    /// <returns></returns>
    StateHistory GetStateByTextsTypeOrDefault(long chatId, string textTypeValue);

    /// <summary>
    /// Получает первоначальное состояние
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetStateMainAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает предыдущие или первоначальное состояние, у которого есть меню
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetStatePreviousOrMainAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает последнее состояние с меню, из истории состояний
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetLastStateWithMenuAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние для заблокированных пользователей
    /// </summary>
    /// <returns></returns>
    StateHistory GetLockState();

    /// <summary>
    /// Получает забиндинное состояние или null
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetBindStateOrNullAsync(long chatId, CancellationToken cancellationToken);
}