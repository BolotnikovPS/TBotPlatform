using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.State;

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
    ChatUpdate ChatUpdate { get; }

    /// <summary>
    /// Информация о состоянии входящей кнопки inline меню
    /// </summary>
    MarkupNextState MarkupNextState { get; }

    /// <summary>
    /// Устанавливает необходимость зафиксировать состояние
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task BindStateAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Снимает необходимость зафиксировать состояние
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnBindStateAsync(CancellationToken cancellationToken);
}