using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Queues;

public class DelayQueueItem
{
    public required string BotName { get; set; }

    public required Func<IStateContextMinimal, Task<Message>> Value { get; set; }

    public required long ChatId { get; set; }

    public DateTime ReadyTime { get; set; }
}
