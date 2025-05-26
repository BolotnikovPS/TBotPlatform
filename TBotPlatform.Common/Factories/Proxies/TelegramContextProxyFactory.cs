using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Abstractions.Factories.Proxies;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Common.Factories.Proxies;

internal class TelegramContextProxyFactory(ILogger<TelegramContextProxyFactory> logger, IHttpClientFactory clientFactory, IServiceScopeFactory serviceScopeFactory)
    : ITelegramContextProxyFactory
{
    public ITelegramContextProxy CreateTelegramContextProxy(TelegramProxySettings telegramProxySetting)
    {
        var settings = new TelegramSettings
        {
            Token = telegramProxySetting.Token,
            ProtectContent = telegramProxySetting.ProtectContent,
        };

        var client = clientFactory.CreateClient(telegramProxySetting.GetHashCode().ToString());

        var scope = serviceScopeFactory.CreateAsyncScope();
        var telegramContextLog = scope.ServiceProvider.GetRequiredService<ITelegramContextLog>();

        logger.LogDebug("Выполняем запрос для токена {token}", settings.Token);

        return new TelegramContextProxy(
            scope,
            client,
            settings,
            telegramContextLog,
            telegramProxySetting.BotPrefixName
            );
    }
}