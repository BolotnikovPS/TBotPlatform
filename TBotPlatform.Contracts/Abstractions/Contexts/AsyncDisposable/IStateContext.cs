using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;

namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

public interface IStateContext : IStateContextMinimal
{
    /// <summary>
    /// Признак необходимости отправки меню
    /// </summary>
    /// <returns></returns>
    bool IsForceReplyLastMenu { get; }

    /// <summary>
    /// Информация о сообщении из чата
    /// </summary>
    /// <returns></returns>
    ChatUpdate ChatUpdate { get; }

    /// <summary>
    /// Информация о состоянии входящей кнопки inline меню
    /// </summary>
    /// <returns></returns>
    MarkupNextState MarkupNextState { get; }

    /// <summary>
    /// Устанавливает необходимость отправки меню
    /// </summary>
    void SetNeedIsForceReplyLastMenu();

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