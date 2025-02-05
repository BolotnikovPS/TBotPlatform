namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.Proxies;

public interface ITelegramContextProxy : ITelegramContext, IAsyncDisposable
{
    string BotPrefixName { get; }
}