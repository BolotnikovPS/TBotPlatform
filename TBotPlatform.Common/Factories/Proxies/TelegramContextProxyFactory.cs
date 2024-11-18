using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Abstractions.Factories.Proxies;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Common.Factories.Proxies;

internal class TelegramContextProxyFactory(ILogger<TelegramContextProxyFactory> logger, HttpClient client, ITelegramContextLog telegramContextLog) 
    : ITelegramContextProxyFactory
{
    public ITelegramContextProxy CreateTelegramContextProxy(TelegramProxySettings telegramProxySetting)
    {
        var settings = new TelegramSettings
        {
            Token = telegramProxySetting.Token,
            ProtectContent = telegramProxySetting.ProtectContent,
            BusinessConnectionId = telegramProxySetting.BusinessConnectionId,
        };

        logger.LogDebug($"Выполняем запрос для токена {settings.Token}");

        return new TelegramContextProxy(client, settings, telegramContextLog, telegramProxySetting.BotPrefixName);
    }

    public async Task<T> MakeRequestAsync<T>(TelegramProxySettings telegramProxySetting, Func<ITelegramContext, Task<T>> request, CancellationToken cancellationToken)
    {
        var settings = new TelegramSettings
        {
            Token = telegramProxySetting.Token,
            ProtectContent = telegramProxySetting.ProtectContent,
            BusinessConnectionId = telegramProxySetting.BusinessConnectionId,
        };

        logger.LogDebug($"Выполняем запрос для токена {settings.Token}");

        await using var context = new TelegramContextProxy(client, settings, telegramContextLog, telegramProxySetting.BotPrefixName);

        return await request.Invoke(context);
    }

    public async Task<T> MakeRequestAsync<T>(TelegramProxySettings telegramProxySetting, Func<ITelegramContext, T> request, CancellationToken cancellationToken)
    {
        var settings = new TelegramSettings
        {
            Token = telegramProxySetting.Token,
            ProtectContent = telegramProxySetting.ProtectContent,
            BusinessConnectionId = telegramProxySetting.BusinessConnectionId,
        };

        logger.LogDebug($"Выполняем запрос для токена {settings.Token}");

        await using var context = new TelegramContextProxy(client, settings, telegramContextLog, telegramProxySetting.BotPrefixName);

        return request.Invoke(context);
    }
}