#nullable enable
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TBotPlatform.Common.BackgroundServices;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Queues;
using TBotPlatform.Contracts.Queues;
using Telegram.Bot.Types;

namespace TBotPlatform.Tests.Common.BackgroundServices;

/// <summary>
/// Checks the delayed message background service: dequeuing an item, invoking the handler,
/// releasing the state context scope, and completing correctly on cancellation.
/// </summary>
/// <remarks>
/// ExecuteAsync is invoked directly (via reflection): the Microsoft.Extensions.Hosting.Abstractions
/// package is not referenced by the test project, so the base BackgroundService members are unavailable.
/// </remarks>
[TestFixture]
public class TelegramDelayHostedServiceTests
{
    private const string BotName = "delay-bot";
    private const long ChatId = 4242;

    private Mock<IDelayQueue> _delayQueue = null!;
    private Mock<IStateContextFactory> _stateContextFactory = null!;
    private Mock<IStateContextMinimal> _stateContext = null!;
    private Mock<IAsyncDisposable> _stateContextDisposable = null!;
    private ServiceProvider _provider = null!;

    [SetUp]
    public void SetUp()
    {
        _delayQueue = new Mock<IDelayQueue>();
        _stateContextFactory = new Mock<IStateContextFactory>();
        _stateContext = new Mock<IStateContextMinimal>();
        _stateContextDisposable = _stateContext.As<IAsyncDisposable>();

        _stateContextDisposable
            .Setup(x => x.DisposeAsync())
            .Returns(ValueTask.CompletedTask);

        _stateContextFactory
            .Setup(x => x.GetStateContext(BotName, ChatId))
            .Returns(_stateContext.Object);

        var services = new ServiceCollection();
        services.AddSingleton(_stateContextFactory.Object);
        _provider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown() => _provider.Dispose();

    [Test]
    public async Task ExecuteAsync_WhenRequestIsDequeued_InvokesRequestAndDisposesStateContext()
    {
        var disposed = new TaskCompletionSource();
        SetUpDisposeSignal(disposed);

        var invokeCount = 0;
        var dequeueCount = 0;
        var secondDequeue = new TaskCompletionSource();

        _delayQueue
            .Setup(x => x.Dequeue(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(cancellationToken => Interlocked.Increment(ref dequeueCount) == 1
                ? Task.FromResult(CreateItem(_ =>
                {
                    invokeCount++;

                    return Task.FromResult(new Message());
                }))
                : WaitForCancellation(cancellationToken, secondDequeue));

        var service = CreateService();
        using var cts = new CancellationTokenSource();

        var executeTask = StartExecute(service, cts.Token);

        await disposed.Task.WaitAsync(TimeSpan.FromSeconds(10));
        await secondDequeue.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cts.Cancel();
        await CompleteExecuteAsync(executeTask);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(invokeCount, Is.EqualTo(1));
            Assert.That(disposed.Task.IsCompleted, Is.True);
            _stateContextFactory.Verify(x => x.GetStateContext(BotName, ChatId), Times.Once);
            _delayQueue.Verify(x => x.Dequeue(It.IsAny<CancellationToken>()), Times.AtLeast(2));
        }

        _stateContextDisposable.Verify(x => x.DisposeAsync(), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WhenRequestThrows_ReleasesStateContextAndContinuesDequeuing()
    {
        var disposed = new TaskCompletionSource();
        SetUpDisposeSignal(disposed);

        var dequeueCount = 0;
        var secondDequeue = new TaskCompletionSource();

        _delayQueue
            .Setup(x => x.Dequeue(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(cancellationToken => Interlocked.Increment(ref dequeueCount) == 1
                ? Task.FromResult(CreateItem(_ => throw new InvalidOperationException("boom")))
                : WaitForCancellation(cancellationToken, secondDequeue));

        var logger = new Mock<ILogger<TelegramDelayHostedService>>();
        var service = CreateService(logger.Object);
        using var cts = new CancellationTokenSource();

        var executeTask = StartExecute(service, cts.Token);

        await disposed.Task.WaitAsync(TimeSpan.FromSeconds(10));
        await secondDequeue.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cts.Cancel();
        await CompleteExecuteAsync(executeTask);

        _delayQueue.Verify(x => x.Dequeue(It.IsAny<CancellationToken>()), Times.AtLeast(2));
        _stateContextDisposable.Verify(x => x.DisposeAsync(), Times.Once);
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
            Times.Once
            );
    }

    [Test]
    public async Task ExecuteAsync_WhenCancelledBeforeRequestArrives_StopsWithoutThrowing()
    {
        var dequeued = new TaskCompletionSource();

        _delayQueue
            .Setup(x => x.Dequeue(It.IsAny<CancellationToken>()))
            .Callback(() => dequeued.TrySetResult())
            .Returns((CancellationToken ct) => WaitForCancellation(ct));

        var service = CreateService();
        using var cts = new CancellationTokenSource();

        var executeTask = StartExecute(service, cts.Token);

        await dequeued.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cts.Cancel();
        await CompleteExecuteAsync(executeTask);

        using (Assert.EnterMultipleScope())
        {
            _delayQueue.Verify(x => x.Dequeue(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            _stateContextFactory.Verify(x => x.GetStateContext(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
        }
    }

    [Test]
    public async Task ExecuteAsync_WhenCancelledWhileRequestIsInFlight_ReleasesStateContext()
    {
        var invoked = new TaskCompletionSource();
        var gate = new TaskCompletionSource();
        var disposed = new TaskCompletionSource();
        SetUpDisposeSignal(disposed);

        var dequeueCount = 0;

        _delayQueue
            .Setup(x => x.Dequeue(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(cancellationToken => Interlocked.Increment(ref dequeueCount) == 1
                ? Task.FromResult(CreateItem(async _ =>
                {
                    invoked.TrySetResult();
                    await gate.Task;

                    return new Message();
                }))
                : WaitForCancellation(cancellationToken));

        var service = CreateService();
        using var cts = new CancellationTokenSource();

        var executeTask = StartExecute(service, cts.Token);

        await invoked.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cts.Cancel();
        gate.SetResult();

        await disposed.Task.WaitAsync(TimeSpan.FromSeconds(10));
        await CompleteExecuteAsync(executeTask);

        _stateContextDisposable.Verify(x => x.DisposeAsync(), Times.Once);
    }

    private void SetUpDisposeSignal(TaskCompletionSource signal)
        => _stateContextDisposable
            .Setup(x => x.DisposeAsync())
            .Callback(() => signal.TrySetResult())
            .Returns(ValueTask.CompletedTask);

    private TelegramDelayHostedService CreateService(ILogger<TelegramDelayHostedService>? logger = null)
        => new(logger ?? NullLogger<TelegramDelayHostedService>.Instance, _delayQueue.Object, _provider);

    private static Task StartExecute(TelegramDelayHostedService service, CancellationToken cancellationToken)
        => (Task)typeof(TelegramDelayHostedService)
                 .GetMethod("ExecuteAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
                 .Invoke(service, [cancellationToken])!;

    /// <summary>
    /// Waits for ExecuteAsync to finish, treating cancellation as the expected way the service stops.
    /// </summary>
    private static async Task CompleteExecuteAsync(Task executeTask)
    {
        try
        {
            await executeTask.WaitAsync(TimeSpan.FromSeconds(10));
        }
        catch (OperationCanceledException)
        {
            // Expected termination due to token cancellation.
        }
    }

    private static DelayQueueItem CreateItem(Func<IStateContextMinimal, Task<Message>> value) => new()
    {
        BotName = BotName,
        ChatId = ChatId,
        Value = value,
    };

    /// <summary>
    /// Returns only when the token is cancelled: simulates an empty delayed-message queue.
    /// </summary>
    private static async Task<DelayQueueItem> WaitForCancellation(
        CancellationToken cancellationToken,
        TaskCompletionSource? signal = null
        )
    {
        signal?.TrySetResult();

        await Task.Delay(Timeout.Infinite, cancellationToken);

        return null!;
    }
}
