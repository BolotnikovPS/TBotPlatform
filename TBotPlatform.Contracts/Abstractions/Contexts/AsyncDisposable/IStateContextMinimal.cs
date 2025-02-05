namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

public interface IStateContextMinimal : IStateContextBase, IAsyncDisposable
{
    /// <summary>
    /// Получает OperationGuid текущих пулов запросов к telegram
    /// </summary>
    /// <returns></returns>
    Guid CurrentOperation { get; }

    /// <summary>
    /// Получает контекст для работы с telegram напрямую
    /// </summary>
    /// <returns></returns>
    ITelegramContext TelegramContext { get; }
}