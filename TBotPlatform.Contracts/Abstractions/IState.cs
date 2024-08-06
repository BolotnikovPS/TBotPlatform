using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots.UserBases;

namespace TBotPlatform.Contracts.Abstractions;

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
    Task HandleAsync(IStateContext context, T user, CancellationToken cancellationToken);

    /// <summary>
    /// Запускает обработку логики состояния после HandleAsync
    /// </summary>
    /// <param name="context">Контекст для обработки сообщения</param>
    /// <param name="user">Пользователь</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleCompleteAsync(IStateContext context, T user, CancellationToken cancellationToken);

    /// <summary>
    /// Запускает обработку логики состояния в случая ошибки
    /// </summary>
    /// <param name="context">Контекст для обработки сообщения</param>
    /// <param name="user">Пользователь</param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleErrorAsync(IStateContext context, T user, Exception exception, CancellationToken cancellationToken);
}