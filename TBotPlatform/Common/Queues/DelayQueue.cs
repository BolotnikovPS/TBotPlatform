using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Queues;
using TBotPlatform.Contracts.Queues;
using TBotPlatform.Extension;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Queues;

internal class DelayQueue : IDelayQueue
{
    private readonly List<DelayQueueItem> _items = [];
    private readonly object _lock = new();
    private TaskCompletionSource _signal = CreateSignal();

    public IResult Enqueue(string botName, long chatId, TimeSpan delay, Func<IStateContextMinimal, Task<Message>> item)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(botName);
            chatId.ThrowIfInvalidChatId();
            ArgumentNullException.ThrowIfNull(item);

            var dateTimeNow = DateTime.UtcNow;
            var readyTime = dateTimeNow.Add(delay);

            if (dateTimeNow >= readyTime)
            {
                throw new InvalidOperationException("Будущее время отправки сообщения меньше или равно текущему.");
            }

            lock (_lock)
            {
                _items.Add(new()
                {
                    BotName = botName,
                    Value = item,
                    ChatId = chatId,
                    ReadyTime = readyTime,
                });

                _items.Sort(static (left, right) => left.ReadyTime.CompareTo(right.ReadyTime));
                Pulse();
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ErrorResult.Failure(ex.Message));
        }
    }

    public async Task<DelayQueueItem> Dequeue(CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            TimeSpan wait;
            Task signalTask;

            lock (_lock)
            {
                var now = DateTime.UtcNow;
                var ready = _items.Find(i => i.ReadyTime <= now);
                if (ready is not null)
                {
                    _items.Remove(ready);
                    return ready;
                }

                wait = _items.Count == 0
                    ? Timeout.InfiniteTimeSpan
                    : _items[0].ReadyTime - now;

                if (wait < TimeSpan.Zero)
                {
                    wait = TimeSpan.Zero;
                }

                signalTask = _signal.Task;
            }

            var delayTask = wait == Timeout.InfiniteTimeSpan
                ? Task.Delay(Timeout.Infinite, cancellationToken)
                : Task.Delay(wait, cancellationToken);

            await Task.WhenAny(signalTask, delayTask).ConfigureAwait(false);
        }
    }

    private void Pulse()
    {
        _signal.TrySetResult();
        _signal = CreateSignal();
    }

    private static TaskCompletionSource CreateSignal()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);
}
