﻿using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Abstractions.State;

public interface IState<in T>
    where T : UserBase
{
    /// <summary>
    /// Запускает обработку логики состояния
    /// </summary>
    /// <param name="context">Контекст для обработки сообщения</param>
    /// <param name="user">Пользователь</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Handle(IStateContext context, T user, CancellationToken cancellationToken);

    /// <summary>
    /// Запускает обработку логики состояния после HandleAsync
    /// </summary>
    /// <param name="context">Контекст для обработки сообщения</param>
    /// <param name="user">Пользователь</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleComplete(IStateContext context, T user, CancellationToken cancellationToken);

    /// <summary>
    /// Запускает обработку логики состояния в случая ошибки
    /// </summary>
    /// <param name="context">Контекст для обработки сообщения</param>
    /// <param name="user">Пользователь</param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleError(IStateContext context, T user, Exception exception, CancellationToken cancellationToken);
}