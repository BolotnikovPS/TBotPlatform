using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Queues;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Queues;

public interface IDelayQueue
{
    void Enqueue(string botName, long chatId, TimeSpan delay, Func<IStateContextMinimal, Task<Message>> item);

    Task<DelayQueueItem> Dequeue(CancellationToken cancellationToken);
}
