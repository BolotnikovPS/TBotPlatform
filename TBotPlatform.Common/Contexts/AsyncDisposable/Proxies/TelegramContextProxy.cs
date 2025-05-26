using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Common.Contexts.AsyncDisposable.Proxies;

internal class TelegramContextProxy(
    AsyncServiceScope scope,
    HttpClient client,
    TelegramSettings telegramSettings,
    ITelegramContextLog telegramContextLog,
    string botPrefixName
    ) : TelegramContext(client, telegramSettings, telegramContextLog), ITelegramContextProxy
{
    private readonly HttpClient _client = client;

    public string BotPrefixName => botPrefixName;

    public new async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        _client.Dispose();
        await scope.DisposeAsync();
    }
}