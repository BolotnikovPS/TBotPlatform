using TBotPlatform.Contracts.Bots;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateFactory : IStateBindFactory
{
    /// <summary>
    /// Проверяет наличие состояния
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="nameOfState">Название состояния</param>
    /// <returns></returns>
    bool HasState(string botName, string nameOfState);

    /// <summary>
    /// Проверяет наличие состояниq
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="nameOfStates">Название состояний</param>
    /// <returns></returns>
    bool HasStates(string botName, string[] nameOfStates);

    /// <summary>
    /// Получает состояние по его названию или /start
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="nameOfState">Название состояния</param>
    /// <returns></returns>
    IResult<StateHistory> GetStateByNameOrDefault(string botName, string nameOfState = "");

    /// <summary>
    /// Получает состояние по типу кнопки или /start
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="buttonTypeValue">Тип кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult<StateHistory>> GetStateByButtonsTypeOrDefault(string botName, long chatId, string buttonTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние по типу команды или /start
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="commandTypeValue">Тип команды</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult<StateHistory>> GetStateByCommandsTypeOrDefault(string botName, long chatId, string commandTypeValue, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние по типу текста или /start
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="textTypeValue">Тип текста</param>
    /// <returns></returns>
    IResult<StateHistory> GetStateByTextsTypeOrDefault(string botName, long chatId, string textTypeValue);

    /// <summary>
    /// Получает первоначальное состояние или /start
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult<StateHistory>> GetStateMain(string botName, long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает предыдущие или первоначальное состояние, у которого есть меню или /start
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult<StateHistory>> GetStatePreviousOrMain(string botName, long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает последнее состояние с меню, из истории состояний
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult<StateHistory>> GetLastStateWithMenu(string botName, long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает состояние для заблокированных пользователей
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <returns></returns>
    IResult<StateHistory> GetLockState(string botName);

    /// <summary>
    /// Получает состояние для заблокированных пользователей
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <returns></returns>
    IResult<StateHistory> GetRegistrationState(string botName);
}