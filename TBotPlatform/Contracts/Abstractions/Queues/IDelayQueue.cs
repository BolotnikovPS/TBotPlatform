using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Queues;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Queues;

public interface IDelayQueue
{
    /// <summary>
    /// Adds a message to the queue
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">ID of the chat to interact with</param>
    /// <param name="delay">Delay time for sending the message</param>
    /// <param name="item">Request for sending the message</param>
    IResult Enqueue(string botName, long chatId, TimeSpan delay, Func<IStateContextMinimal, Task<Message>> item);

    /// <summary>
    /// Gets a message from the queue
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task<DelayQueueItem> Dequeue(CancellationToken cancellationToken);
}
