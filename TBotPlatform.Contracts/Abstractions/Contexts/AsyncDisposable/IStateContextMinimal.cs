namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

public interface IStateContextMinimal : IStateContextProxyMinimal
{
    /// <summary>
    /// Получает OperationGuid текущих пулов запросов к telegram
    /// </summary>
    /// <returns></returns>
    Guid GetCurrentOperation();

    /// <summary>
    /// Получает контекст для работы с telegram напрямую
    /// </summary>
    /// <returns></returns>
    ITelegramContext GetTelegramContext();
}