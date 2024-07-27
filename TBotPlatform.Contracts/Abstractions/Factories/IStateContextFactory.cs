﻿using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateContextFactory
{
    /// <summary>
    /// Создание контекста состояния
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContext> CreateStateContextAsync<T>(T user, CancellationToken cancellationToken)
        where T : UserBase;

    /// <summary>
    /// Создание контекста состояния и вызов состояния
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContext> CreateStateContextAsync<T>(
        T user,
        StateHistory<T> stateHistory,
        Update update,
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
        StateHistory<T> stateHistory,
        Update update,
        MarkupNextState markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase;
}