using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Contexts.AsyncDisposable;
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

        return new TelegramContextProxy(client, settings, telegramContextLog);
    }

    public async Task<T> MakeBotRequestAsync<T>(string botToken, bool protectContent, Func<ITelegramContext, Task<T>> request, CancellationToken cancellationToken)
    {
        var settings = new TelegramSettings
        {
            Token = botToken,
            ProtectContent = protectContent,
        };

        logger.LogDebug($"Выполняем запрос для токена {botToken}");

        await using var context = new TelegramContextProxy(client, settings, telegramContextLog);

        return await request.Invoke(context);
    }
}