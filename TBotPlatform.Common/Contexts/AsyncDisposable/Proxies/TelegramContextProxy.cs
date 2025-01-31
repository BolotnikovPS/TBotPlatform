using Microsoft.Extensions.Options;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Common.Contexts.AsyncDisposable.Proxies;

internal class TelegramContextProxy(HttpClient client, TelegramSettings telegramSettings, ITelegramContextLog telegramContextLog, string botPrefixName)
    : TelegramContext(client, telegramSettings, telegramContextLog), ITelegramContextProxy
{
    public string GetBotPrefixName() => botPrefixName;
}