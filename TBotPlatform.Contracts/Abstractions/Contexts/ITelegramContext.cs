#nullable enable
using Telegram.Bot;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface ITelegramContext : ITelegramBotClient
{
    /// <summary>
    /// Получает OperationGuid текущих пулов запросов к telegram
    /// </summary>
    /// <returns></returns>
    Guid CurrentOperation { get; }
}