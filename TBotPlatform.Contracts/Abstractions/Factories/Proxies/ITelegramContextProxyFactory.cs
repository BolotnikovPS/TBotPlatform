using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Contracts.Abstractions.Factories.Proxies;

public interface ITelegramContextProxyFactory
{
    /// <summary>
    /// Создает прокси контекст для запросов в телеграм
    /// </summary>
    /// <param name="telegramProxySetting">Настройки бота</param>
    /// <returns></returns>
    ITelegramContextProxy CreateTelegramContextProxy(TelegramProxySettings telegramProxySetting);
}