#nullable enable
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Users;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateContextFactory
{
    /// <summary>
    /// Создание контекста состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <returns></returns>
    IStateContextMinimal GetStateContext<T>(string botName, T user) where T : UserBase;

    /// <summary>
    /// Создание контекста состояния
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата с которым будем взаимодействовать</param>
    /// <returns></returns>
    IStateContextMinimal GetStateContext(string botName, long chatId);

    /// <summary>
    /// Создание контекста состояния и вызов состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContextMinimal> CreateStateContext<T>(string botName, T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase;

    /// <summary>
    /// Создание контекста состояния и вызов состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="markupNextState">Данные с кнопки inline</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContextMinimal> CreateStateContext<T>(
        string botName, 
        T user,
        StateHistory stateHistory,
        Update update,
        MarkupNextState? markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase;
}