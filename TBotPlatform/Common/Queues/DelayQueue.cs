using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Queues;
using TBotPlatform.Contracts.Queues;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Queues;

internal class DelayQueue : IDelayQueue
{
    private List<DelayQueueItem> items = [];

    private readonly object _lock = new();

    public void Enqueue(string botName, long chatId, TimeSpan delay, Func<IStateContextMinimal, Task<Message>> item)
    {
        ArgumentNullException.ThrowIfNull(botName);
        ArgumentNullException.ThrowIfNull(chatId);

        var dateTimeNow = DateTime.UtcNow;
        var readyTime = dateTimeNow.Add(delay);

        if (dateTimeNow >= readyTime)
        {
            throw new Exception("Будущее время отправки сообщения меньше или равно текущему.");
        }

        lock (_lock)
        {
            items.Add(new()
            {
                BotName = botName,
                Value = item,
                ChatId = chatId,
                ReadyTime = readyTime,
            });

            items = [.. items.OrderBy(item => item.ReadyTime)];
        }
    }

    public async Task<DelayQueueItem> Dequeue(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            var item = items.FirstOrDefault(i => i.ReadyTime <= now);
            if (item == null)
            {
                await Task.Delay(1000, cancellationToken);
                continue;
            }

            items.Remove(item);
            return item;
        }

        throw new OperationCanceledException();
    }
}
