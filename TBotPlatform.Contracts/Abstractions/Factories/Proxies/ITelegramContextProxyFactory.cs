using TBotPlatform.Contracts.Abstractions.Contexts;
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

    /// <summary>
    /// Делает запрос в телеграм с прокси контекстом
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="telegramProxySetting">Настройки бота</param>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> MakeRequestAsync<T>(TelegramProxySettings telegramProxySetting, Func<ITelegramContext, Task<T>> request, CancellationToken cancellationToken);

    /// <summary>
    /// Делает запрос в телеграм с прокси контекстом
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="telegramProxySetting">Настройки бота</param>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> MakeRequestAsync<T>(TelegramProxySettings telegramProxySetting, Func<ITelegramContext, T> request, CancellationToken cancellationToken);
}