using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories.Proxies;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Common.Factories.Proxies;

internal class TelegramContextProxyFactory(ILogger<TelegramContextProxyFactory> logger, HttpClient client, ITelegramContextLog telegramContextLog) 
    : ITelegramContextProxyFactory
{
    public ITelegramContextProxy CreateTelegramContextProxy(string botToken, bool protectContent)
    {
        var settings = new TelegramSettings
        {
            Token = botToken,
            ProtectContent = protectContent,
        };

        logger.LogDebug($"Выполняем запрос для токена {botToken}");

        return new TelegramContext(client, settings, telegramContextLog);
    }

    public Task<T> MakeBotRequestAsync<T>(string botToken, bool protectContent, Func<ITelegramContext, Task<T>> request, CancellationToken cancellationToken)
    {
        var settings = new TelegramSettings
        {
            Token = botToken,
            ProtectContent = protectContent,
        };

        logger.LogDebug($"Выполняем запрос для токена {botToken}");

        var context = new TelegramContext(client, settings, telegramContextLog);

        return request.Invoke(context);
    }
}