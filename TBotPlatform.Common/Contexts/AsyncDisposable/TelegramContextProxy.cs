using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal class TelegramContextProxy(HttpClient client, TelegramSettings telegramSettings, ITelegramContextLog telegramContextLog)
    : TelegramContext(client, telegramSettings!, telegramContextLog), ITelegramContextProxy;