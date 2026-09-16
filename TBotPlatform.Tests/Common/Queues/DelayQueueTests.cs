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
    public void Enqueue_WhenChatIdInvalid_ReturnsFailure()
    {
        var result = _queue.Enqueue("bot", 0L, TimeSpan.FromSeconds(5), _ => Task.FromResult(new Message()));

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
        Assert.That(_queue.Enqueue("bot1", 100L, TimeSpan.FromSeconds(1), _ => Task.FromResult(new Message())).IsSuccess, Is.True);
    }

    [Test]
    public async Task Dequeue_WhenItemReady_ReturnsItem()
    {
        var delay = TimeSpan.FromMilliseconds(10);
        var message = new Message();
        _queue.Enqueue("bot", 1L, delay, _ => Task.FromResult(message));

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

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        var first = await _queue.Dequeue(cts.Token);
        Assert.That(first.ChatId, Is.EqualTo(2L));
    }

    [Test]
    public async Task Enqueue_Concurrent_DoesNotLoseItems()
    {
        var tasks = Enumerable.Range(1, 20).Select(i => Task.Run(() =>
            _queue.Enqueue("bot", i + 10L, TimeSpan.FromMilliseconds(5), _ => Task.FromResult(new Message()))));

        await Task.WhenAll(tasks);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var seen = new HashSet<long>();
        for (var i = 0; i < 20; i++)
        {
            var item = await _queue.Dequeue(cts.Token);
            seen.Add(item.ChatId);
        }

        Assert.That(seen, Has.Count.EqualTo(20));
    }
}
