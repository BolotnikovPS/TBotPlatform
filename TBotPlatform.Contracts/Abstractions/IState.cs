using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Contracts.Abstractions;

public interface IState<in T>
where T : UserBase
{
    /// <summary>
    /// Запускает обработку логики состояния
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleAsync(IStateContext<T> context, CancellationToken cancellationToken);

    /// <summary>
    /// Запускает обработку логики состояния в случая ошибки
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleErrorAsync(IStateContext<T> context, Exception exception, CancellationToken cancellationToken);
}