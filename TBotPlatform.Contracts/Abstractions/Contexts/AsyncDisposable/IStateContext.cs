using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.State;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

public interface IStateContext : IStateContextMinimal
{
    /// <summary>
    /// Результат работы состояния
    /// </summary>
    StateResult StateResult { get; set; }

    /// <summary>
    /// Информация о сообщении из чата
    /// </summary>
    Update ChatUpdate { get; }

    /// <summary>
    /// Информация о состоянии входящей кнопки inline меню
    /// </summary>
    MarkupNextState MarkupNextState { get; }

    /// <summary>
    /// Осуществляет запрос в чат, отличающийся от заданного в <see cref="IStateContextFactory"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="newChatId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<T> MakeRequestToOtherChat<T>(long newChatId, Func<IStateContextMinimal, Task<T>> request);

    /// <summary>
    /// Устанавливает необходимость зафиксировать состояние
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task BindState(CancellationToken cancellationToken);

    /// <summary>
    /// Снимает необходимость зафиксировать состояние
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnBindState(CancellationToken cancellationToken);
}