using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

namespace TBotPlatform.Contracts.Abstractions.Factories.Proxies;

public interface ITelegramContextProxyFactory
{
    ITelegramContextProxy CreateTelegramContextProxy(string botToken, bool protectContent);
    Task<T> MakeBotRequestAsync<T>(string botToken, bool protectContent, Func<ITelegramContext, Task<T>> request, CancellationToken cancellationToken);
}