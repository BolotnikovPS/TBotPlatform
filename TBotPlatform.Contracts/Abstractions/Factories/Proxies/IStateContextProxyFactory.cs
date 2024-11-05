﻿#nullable enable
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts.Proxies;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.State;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Factories.Proxies;

public interface IStateContextProxyFactory
{
    public void SetBot(ITelegramContextProxy telegramContextProxy);
    /// <summary>
    /// Создание контекста состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <returns></returns>
    /// 
    IStateContextProxyMinimal CreateStateContext<T>(T user) where T : UserBase;

    /// <summary>
    /// Создание контекста состояния
    /// </summary>
    /// <param name="chatId">Id чата с которым будем взаимодействовать</param>
    /// <returns></returns>
    IStateContextProxyMinimal CreateStateContext(long chatId);

    /// <summary>
    /// Создание контекста состояния и вызов состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContextProxyMinimal> CreateStateContextAsync<T>(T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase;

    /// <summary>
    /// Создание контекста состояния и вызов состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="chatUpdate">Форматированное сообщение с telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContextProxyMinimal> CreateStateContextAsync<T>(T user, StateHistory stateHistory, ChatUpdate chatUpdate, CancellationToken cancellationToken)
        where T : UserBase;

    /// <summary>
    /// Создание контекста состояния и вызов состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="chatUpdate">Форматированное сообщение с telegram</param>
    /// <param name="markupNextState">Данные с кнопки inline</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IStateContextProxyMinimal> CreateStateContextAsync<T>(
        T user,
        StateHistory stateHistory,
        ChatUpdate chatUpdate,
        MarkupNextState? markupNextState,
        CancellationToken cancellationToken
        )
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
    Task<IStateContextProxyMinimal> CreateStateContextAsync<T>(
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
    StateResult? GetStateResult(IStateContextProxyMinimal stateContext);
}