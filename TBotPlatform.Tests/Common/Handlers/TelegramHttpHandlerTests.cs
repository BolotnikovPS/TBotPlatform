#nullable enable
using System.Net;
using System.Net.Http.Headers;
using ComposableAsync;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.Constant;

namespace TBotPlatform.Tests.Common.Handlers;

/// <summary>
/// Tests <see cref="TelegramHttpHandler"/>: extracting the operation's service header,
/// capturing Retry-After, verbose payload logging, and the error log level.
/// </summary>
[TestFixture]
public class TelegramHttpHandlerTests
{
    private const string RequestUriValue = "https://api.telegram.org/bot1:token/getMe";

    [Test]
    public async Task SendAsync_WhenOperationHeaderPresent_RemovesHeaderAndLogsOperationGuid()
    {
        var operationGuid = Guid.NewGuid();
        var logger = new CollectingLogger<TelegramHttpHandler>();
        var stub = new StubHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        using var client = CreateClient(logger, stub, CreateBotSetting());
        using var request = CreateRequest(HttpMethod.Get);
        request.Headers.Add(DefaultHeadersConstant.ContextOperation, operationGuid.ToString());

        var response = await client.SendAsync(request);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(stub.LastRequest, Is.Not.Null);
            Assert.That(stub.LastRequest!.Headers.Contains(DefaultHeadersConstant.ContextOperation), Is.False);
            Assert.That(logger.Entries.Any(x => x.Message.Contains(operationGuid.ToString())), Is.True);
            Assert.That(logger.Entries.Any(x => x.Message.Contains(RequestUriValue)), Is.True);
        }
    }

    [Test]
    public async Task SendAsync_WhenOperationHeaderMissing_LogsEmptyGuidAndDoesNotThrow()
    {
        var logger = new CollectingLogger<TelegramHttpHandler>();
        var stub = new StubHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        using var client = CreateClient(logger, stub, CreateBotSetting());
        using var request = CreateRequest(HttpMethod.Get);

        var response = await client.SendAsync(request);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(logger.Entries, Is.Not.Empty);
            Assert.That(logger.Entries.Any(x => x.Message.Contains(Guid.Empty.ToString())), Is.True);
        }
    }

    [Test]
    public async Task SendAsync_WhenTooManyRequestsWithRetryAfter_LogsRetryAfterDelta()
    {
        var logger = new CollectingLogger<TelegramHttpHandler>();
        var stub = new StubHttpMessageHandler((_, _) =>
        {
            var tooManyRequests = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Headers = { RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(30)) },
            };

            return Task.FromResult(tooManyRequests);
        });

        using var client = CreateClient(logger, stub, CreateBotSetting());
        using var request = CreateRequest(HttpMethod.Post);

        var response = await client.SendAsync(request);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.TooManyRequests));
            Assert.That(logger.Entries.Any(x => x.Message.Contains("00:00:30")), Is.True);
        }
    }

    [Test]
    public async Task SendAsync_WhenVerboseLogEnabled_LogsRequestPayload()
    {
        const string payload = "{\"chat_id\":\"42\"}";

        var logger = new CollectingLogger<TelegramHttpHandler>();
        var stub = new StubHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        using var client = CreateClient(logger, stub, CreateBotSetting(verboseLog: true));
        using var request = CreateRequest(HttpMethod.Post);
        request.Content = new StringContent(payload);

        await client.SendAsync(request);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(logger.Entries.Any(x => x.Level == LogLevel.Debug && x.Message.Contains("Request payload")), Is.True);
            Assert.That(logger.Entries.Any(x => x.Message.Contains(payload)), Is.True);
        }
    }

    [Test]
    public async Task SendAsync_WhenVerboseLogDisabled_DoesNotLogRequestPayload()
    {
        var logger = new CollectingLogger<TelegramHttpHandler>();
        var stub = new StubHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        using var client = CreateClient(logger, stub, CreateBotSetting(verboseLog: false));
        using var request = CreateRequest(HttpMethod.Post);
        request.Content = new StringContent("{\"chat_id\":\"42\"}");

        await client.SendAsync(request);

        Assert.That(logger.Entries.Any(x => x.Message.Contains("Request payload")), Is.False);
    }

    [Test]
    public async Task SendAsync_WhenInnerHandlerThrows_LogsErrorAndRethrows()
    {
        var logger = new CollectingLogger<TelegramHttpHandler>();
        var stub = new StubHttpMessageHandler((_, _) => throw new InvalidOperationException("network failure"));

        using var client = CreateClient(logger, stub, CreateBotSetting());
        using var request = CreateRequest(HttpMethod.Get);

        Assert.ThrowsAsync<InvalidOperationException>(async () => await client.SendAsync(request));

        var lastEntry = logger.Entries.Last();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(lastEntry.Level, Is.EqualTo(LogLevel.Error));
            Assert.That(lastEntry.Exception, Is.TypeOf<InvalidOperationException>());
        }
    }

    private static HttpClient CreateClient(
        ILogger<TelegramHttpHandler> logger,
        StubHttpMessageHandler stub,
        TBotSetting botSetting
        )
    {
        var handler = new TelegramHttpHandler(logger, NullDispatcher.Instance, botSetting)
        {
            InnerHandler = stub,
        };

        return new HttpClient(handler, disposeHandler: true);
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method)
        => new(method, RequestUriValue);

    private static TBotSetting CreateBotSetting(bool verboseLog = false)
        => new()
        {
            BotName = "test",
            Token = "1:token",
            VerboseLog = verboseLog,
        };

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
        : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;

            return handler(request, cancellationToken);
        }
    }

    private sealed class CollectingLogger<T> : ILogger<T>
    {
        public List<LogEntry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
            => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
            )
            => Entries.Add(new LogEntry(logLevel, formatter(state, exception), exception));
    }

    private sealed record LogEntry(LogLevel Level, string Message, Exception? Exception);
}
