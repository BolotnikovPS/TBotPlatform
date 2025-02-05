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
        };

        logger.LogDebug($"Выполняем запрос для токена {settings.Token}");

        return new TelegramContextProxy(client, settings, telegramContextLog, telegramProxySetting.BotPrefixName);
    }
}