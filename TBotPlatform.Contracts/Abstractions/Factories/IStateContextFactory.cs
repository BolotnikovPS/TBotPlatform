using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatMessages;
using TBotPlatform.Contracts.Bots.UserBases;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateContextFactory
{
    /// <summary>
    /// Создание контекста состояния
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <returns></returns>
    IStateContext CreateStateContext<T>(T user) where T : UserBase;

    /// <summary>
    /// Создание контекста состояния и вызов состояния
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContext> CreateStateContextAsync<T>(T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase;

    /// <summary>
    /// Создание контекста состояния и вызов состояния
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="user">Пользователь</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="chatMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContext> CreateStateContextAsync<T>(T user, StateHistory stateHistory, ChatMessage chatMessage, CancellationToken cancellationToken)
        where T : UserBase;

    /// <summary>
    /// Создание контекста состояния и вызов состояния
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="user">Пользователь</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="chatMessage">Сообщение для состояния</param>
    /// <param name="markupNextState">Данные с кнопки inline</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IStateContext> CreateStateContextAsync<T>(
        T user,
        StateHistory stateHistory,
        ChatMessage chatMessage,
        MarkupNextState markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase;

    /// <summary>
    /// Создание контекста состояния и вызов состояния
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="markupNextState">Данные с кнопки inline</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContext> CreateStateContextAsync<T>(
        T user,
        StateHistory stateHistory,
        Update update,
        MarkupNextState markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase;

    /// <summary>
    /// Получает кнопки состояния по вызываемому состоянию
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="user">Пользователь</param>
    /// <param name="stateContext">Контекст состояния</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateMarkupByStateAsync<T>(T user, IStateContext stateContext, StateHistory stateHistory, CancellationToken cancellationToken)
        where T : UserBase;
}