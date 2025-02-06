#nullable enable
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Contracts.State;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateContextFactory
{
    /// <summary>
    /// Создание контекста состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <returns></returns>
    IStateContextMinimal GetStateContext<T>(T user) where T : UserBase;

    /// <summary>
    /// Создание контекста состояния
    /// </summary>
    /// <param name="chatId">Id чата с которым будем взаимодействовать</param>
    /// <returns></returns>
    IStateContextMinimal GetStateContext(long chatId);

    /// <summary>
    /// Создание контекста состояния и вызов состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContextMinimal> CreateStateContext<T>(T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase;

    /// <summary>
    /// Создание контекста состояния и вызов состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="markupNextState">Данные с кнопки inline</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContextMinimal> CreateStateContext<T>(
        T user,
        StateHistory stateHistory,
        Update update,
        MarkupNextState? markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase;

    /// <summary>
    /// Получает результат выполнения состояния
    /// </summary>
    /// <param name="stateContext">Контекст состояния</param>
    /// <returns></returns>
    StateResult? GetStateResult(IStateContextMinimal stateContext);
}