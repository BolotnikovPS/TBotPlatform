#nullable enable
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TBotPlatform.Common.BackgroundServices;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots.Config;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace TBotPlatform.Tests.Common.BackgroundServices;

/// <summary>
/// Tests the Telegram context background service without actual network calls: Telegram Bot API calls are replaced
/// with a mock of <see cref="ITelegramContext"/>, long polling and webhook modes, error handling
/// and disposal of the bot scope on shutdown are verified.
/// </summary>
/// <remarks>
/// ExecuteAsync is called directly (via reflection): the Microsoft.Extensions.Hosting.Abstractions
/// package is not referenced by the test project, so the base BackgroundService members are unavailable.
/// </remarks>
[TestFixture]
public class TelegramContextHostedServiceTests
{
    private const string BotName = "context-bot";

    private Mock<ITelegramContext> _telegramContext = null!;
    private Mock<IAsyncDisposable> _telegramContextDisposable = null!;
    private TBotSetting _setting = null!;
    private ServiceProvider _provider = null!;

    [SetUp]
    public void SetUp()
    {
        _setting = CreateSetting();

        _telegramContext = new Mock<ITelegramContext>();
        _telegramContextDisposable = _telegramContext.As<IAsyncDisposable>();

        _telegramContextDisposable
            .Setup(x => x.DisposeAsync())
            .Returns(ValueTask.CompletedTask);

        _telegramContext
            .Setup(x => x.GetBotSetting())
            .Returns(() => _setting);

        _telegramContext
            .Setup(x => x.SendRequest(It.IsAny<GetMeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = 1, IsBot = true, FirstName = "bot" });

        var services = new ServiceCollection();
        services.AddKeyedScoped<ITelegramContext>(BotName, (_, _) => _telegramContext.Object);
        _provider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown() => _provider.Dispose();

    [Test]
    public async Task ExecuteAsync_WhenBotUsesLongPolling_PollsUpdatesAndStopsOnCancellation()
    {
        var updatesRequested = new TaskCompletionSource();

        _telegramContext
            .Setup(x => x.SendRequest(It.IsAny<GetUpdatesRequest>(), It.IsAny<CancellationToken>()))
            .Callback(() => updatesRequested.TrySetResult())
            .Returns<GetUpdatesRequest, CancellationToken>((_, _) => DelayedEmptyUpdates());

        var service = CreateService([BotName]);
        using var cts = new CancellationTokenSource();

        var executeTask = StartExecute(service, cts.Token);

        await updatesRequested.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cts.Cancel();
        await CompleteExecuteAsync(executeTask);

        using (Assert.EnterMultipleScope())
        {
            _telegramContext.Verify(x => x.SendRequest(It.IsAny<GetMeRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _telegramContext.Verify(x => x.SendRequest(It.IsAny<GetUpdatesRequest>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        _telegramContextDisposable.Verify(x => x.DisposeAsync(), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WhenBotUsesWebhook_SetsWebhookAndDeletesItOnShutdown()
    {
        _setting = CreateSetting(webhookUrl: "https://example.test/hook");

        var webhookSet = new TaskCompletionSource();
        var webhookDeleted = new TaskCompletionSource();

        _telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SetWebhookRequest>(), It.IsAny<CancellationToken>()))
            .Callback(() => webhookSet.TrySetResult())
            .ReturnsAsync(true);

        _telegramContext
            .Setup(x => x.SendRequest(It.IsAny<DeleteWebhookRequest>(), It.IsAny<CancellationToken>()))
            .Callback(() => webhookDeleted.TrySetResult())
            .ReturnsAsync(true);

        var service = CreateService([BotName]);
        using var cts = new CancellationTokenSource();

        var executeTask = StartExecute(service, cts.Token);

        await webhookSet.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cts.Cancel();

        await webhookDeleted.Task.WaitAsync(TimeSpan.FromSeconds(10));
        await CompleteExecuteAsync(executeTask);

        using (Assert.EnterMultipleScope())
        {
            _telegramContext.Verify(x => x.SendRequest(It.IsAny<SetWebhookRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _telegramContext.Verify(x => x.SendRequest(It.IsAny<DeleteWebhookRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _telegramContext.Verify(x => x.SendRequest(It.IsAny<GetUpdatesRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    [Test]
    public async Task ExecuteAsync_WhenGetMeThrows_LogsErrorAndDoesNotPollUpdates()
    {
        var getMeCalled = new TaskCompletionSource();
        var logger = new Mock<ILogger<TelegramContextHostedService>>();

        _telegramContext
            .Setup(x => x.SendRequest(It.IsAny<GetMeRequest>(), It.IsAny<CancellationToken>()))
            .Callback(() => getMeCalled.TrySetResult())
            .ThrowsAsync(new InvalidOperationException("boom"));

        var service = CreateService([BotName], logger.Object);

        var executeTask = StartExecute(service, CancellationToken.None);

        await getMeCalled.Task.WaitAsync(TimeSpan.FromSeconds(10));
        await CompleteExecuteAsync(executeTask);

        using (Assert.EnterMultipleScope())
        {
            _telegramContext.Verify(x => x.SendRequest(It.IsAny<GetUpdatesRequest>(), It.IsAny<CancellationToken>()), Times.Never);
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

        _telegramContextDisposable.Verify(x => x.DisposeAsync(), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WhenWebhookSetupFailsWithApiError_DoesNotRetryAndCompletes()
    {
        _setting = CreateSetting(webhookUrl: "https://example.test/hook");

        _telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SetWebhookRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApiRequestException("bad token", 401));

        var service = CreateService([BotName]);

        var executeTask = StartExecute(service, CancellationToken.None);

        await CompleteExecuteAsync(executeTask);

        using (Assert.EnterMultipleScope())
        {
            _telegramContext.Verify(x => x.SendRequest(It.IsAny<SetWebhookRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _telegramContext.Verify(x => x.SendRequest(It.IsAny<DeleteWebhookRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    [Test]
    public async Task ExecuteAsync_WhenBotIsNotRegisteredInDi_CompletesWithoutThrowing()
    {
        var service = CreateService(["unknown-bot"]);

        var executeTask = StartExecute(service, CancellationToken.None);

        await CompleteExecuteAsync(executeTask);

        _telegramContext.Verify(x => x.SendRequest(It.IsAny<GetMeRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task ExecuteAsync_WhenBotListIsEmpty_DoesNotTouchTelegramContext()
    {
        var service = CreateService([]);

        var executeTask = StartExecute(service, CancellationToken.None);

        await CompleteExecuteAsync(executeTask);

        _telegramContext.VerifyNoOtherCalls();
    }

    private TelegramContextHostedService CreateService(
        IEnumerable<string> bots,
        ILogger<TelegramContextHostedService>? logger = null
        )
        => new(
            logger ?? NullLogger<TelegramContextHostedService>.Instance,
            bots.ToList(),
            _provider,
            Mock.Of<ITelegramUpdateProcessor>());

    private static Task StartExecute(TelegramContextHostedService service, CancellationToken cancellationToken)
        => (Task)typeof(TelegramContextHostedService)
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

    private static async Task<Update[]> DelayedEmptyUpdates()
    {
        // The delay prevents the long polling loop from spinning on empty updates; cancellation is handled by ReceiveAsync.
        await Task.Delay(20, CancellationToken.None);

        return [];
    }

    private static TBotSetting CreateSetting(string? webhookUrl = null) => new()
    {
        BotName = BotName,
        Token = "123:token",
        WebhookUrl = webhookUrl,
    };
}
