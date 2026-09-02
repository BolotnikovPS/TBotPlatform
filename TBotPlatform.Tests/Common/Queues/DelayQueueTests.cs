using TBotPlatform.Common.Queues;
using Telegram.Bot.Types;

namespace TBotPlatform.Tests.Common.Queues;

[TestFixture]
public class DelayQueueTests
{
    private DelayQueue _queue = null!;

    [SetUp]
    public void SetUp() => _queue = new DelayQueue();

    [Test]
    public void Enqueue_WhenBotNameIsNull_ThrowsArgumentNullException()
    {
        var result = _queue.Enqueue(null!, 12345L, TimeSpan.FromSeconds(5), _ => Task.FromResult(new Message()));

        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void Enqueue_WhenDelayIsZero_ThrowsException()
    {
        var result = _queue.Enqueue("bot", 12345L, TimeSpan.Zero, _ => Task.FromResult(new Message()));

        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void Enqueue_WhenDelayIsNegative_ThrowsException()
    {
        var result = _queue.Enqueue("bot", 12345L, TimeSpan.FromSeconds(-1), _ => Task.FromResult(new Message()));

        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void Enqueue_WithValidArguments_AddsItem()
    {
        var delay = TimeSpan.FromMilliseconds(50);
        Message? capturedMessage = null;
        _queue.Enqueue("bot1", 100L, delay, _ =>
        {
            capturedMessage = new Message();
            return Task.FromResult(capturedMessage);
        });

        Assert.DoesNotThrow(() => _queue.Enqueue("bot1", 100L, TimeSpan.FromSeconds(1), _ => Task.FromResult(new Message())));
    }

    [Test]
    public async Task Dequeue_WhenItemReady_ReturnsItem()
    {
        var delay = TimeSpan.FromMilliseconds(10);
        var message = new Message();
        _queue.Enqueue("bot", 1L, delay, _ => Task.FromResult(message));

        await Task.Delay(50);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        var item = await _queue.Dequeue(cts.Token);

        Assert.That(item, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(item.BotName, Is.EqualTo("bot"));
            Assert.That(item.ChatId, Is.EqualTo(1L));
            Assert.That(item.Value, Is.Not.Null);
        }
    }

    [Test]
    public void Dequeue_WhenCancelled_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.ThrowsAsync<OperationCanceledException>(async () => await _queue.Dequeue(cts.Token));
    }

    [Test]
    public async Task Enqueue_OrdersItemsByReadyTime()
    {
        _queue.Enqueue("bot", 1L, TimeSpan.FromSeconds(3), _ => Task.FromResult(new Message()));
        _queue.Enqueue("bot", 2L, TimeSpan.FromMilliseconds(20), _ => Task.FromResult(new Message()));

        await Task.Delay(50);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        var first = await _queue.Dequeue(cts.Token);
        Assert.That(first.ChatId, Is.EqualTo(2L));
    }
}
