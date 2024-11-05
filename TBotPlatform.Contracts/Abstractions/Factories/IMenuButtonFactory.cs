using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IMenuButtonFactory
{
    /// <summary>
    /// Обновляет кнопки основного меню по вызываемому состоянию
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <param name="stateContext">Контекст состояния</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateMainButtonsByStateAsync<T>(T user, IStateContextMinimal stateContext, StateHistory stateHistory, CancellationToken cancellationToken)
        where T : UserBase;

    /// <summary>
    /// Обновляет кнопки основного меню по типу меню
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <param name="stateContext">Контекст состояния</param>
    /// <param name="menuStateType">Тип вызываемого меню</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateMainButtonsByStateAsync<T>(T user, IStateContextMinimal stateContext, Type menuStateType, CancellationToken cancellationToken)
        where T : UserBase;
}