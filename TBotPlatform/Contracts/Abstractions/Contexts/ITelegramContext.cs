#nullable enable
using TBotPlatform.Contracts.Bots.Config;
using Telegram.Bot;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface ITelegramContext : ITelegramBotClient
{
    /// <summary>
    /// Получает OperationGuid текущих пулов запросов к telegram
    /// </summary>
    /// <returns></returns>
    Guid CurrentOperation { get; }

    /// <summary>
    /// Получение настроек контекста бота
    /// </summary>
    /// <returns></returns>
    TelegramSettings GetTelegramSettings();
}