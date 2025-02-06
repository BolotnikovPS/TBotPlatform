using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateFactory : IStateBindFactory
{
    /// <summary>
    /// Получает состояние по его названию или /start
    /// </summary>
    /// <param name="nameOfState">Название состояния</param>
    /// <returns></returns>
    StateHistory GetStateByNameOrDefault(string nameOfState = "");

    /// <summary>
    /// Получает состояние по типу кнопки или /start
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="buttonTypeValue">Тип кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetStateByButtonsTypeOrDefault(long chatId, string buttonTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние по типу команды или /start
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="commandTypeValue">Тип команды</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetStateByCommandsTypeOrDefault(long chatId, string commandTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние по типу текста или /start
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="textTypeValue">Тип текста</param>
    /// <returns></returns>
    StateHistory GetStateByTextsTypeOrDefault(long chatId, string textTypeValue);

    /// <summary>
    /// Получает первоначальное состояние или /start
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetStateMain(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает предыдущие или первоначальное состояние, у которого есть меню или /start
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetStatePreviousOrMain(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает последнее состояние с меню, из истории состояний
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StateHistory> GetLastStateWithMenu(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние для заблокированных пользователей
    /// </summary>
    /// <returns></returns>
    StateHistory LockState { get; }

    /// <summary>
    /// Получает состояние для заблокированных пользователей
    /// </summary>
    /// <returns></returns>
    StateHistory RegistrationState { get; }
}