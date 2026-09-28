#nullable enable
using System.Net;
using System.Text;
using Microsoft.IO;
using Moq;
using NUnit.Framework;
using TBotPlatform.Common.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Statistics;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace TBotPlatform.Tests.Common.Contexts;

/// <summary>
/// Проверяет применение настроек Telegram.Bot (TelegramBotClientOptions) и работу
/// переопределенного SendRequest: ProtectContent, verbose-логирование и скачивание файлов.
/// </summary>
[TestFixture]
public class TelegramContextTests
{
    private const string Token = "123456:TEST_TOKEN";

    private const string MessageResponseJson = """
        {"ok":true,"result":{"message_id":10,"date":1700000000,"chat":{"id":42,"type":"private"},"text":"hello"}}
        """;

    private const string GetFileResponseJson = """
        {"ok":true,"result":{"file_id":"AgAD","file_unique_id":"unique","file_size":4,"file_path":"photos/file_1.jpg"}}
        """;

    [Test]
    public async Task SendRequest_WhenProtectContentAndVerboseLogEnabled_SetsFlagAndWritesFullLog()
    {
        var handler = new StubHttpMessageHandler(MessageResponseJson);
        var log = CreateLogMock();

        var setting = new TBotSetting
        {
            BotName = "bot",
            Token = Token,
            ProtectContent = true,
            VerboseLog = true,
        };

        await using var context = CreateContext(handler, setting, log.Object);

        var request = new SendMessageRequest
        {
            ChatId = 42,
            Text = "hello",
        };

        var message = await context.SendRequest(request, CancellationToken.None);

        Assert.That(message.MessageId, Is.EqualTo(10));
        Assert.That(request.ProtectContent, Is.True, "ProtectContent должен проставляться в запрос");
        Assert.That(handler.RequestBodies.Single(), Does.Contain("protect_content"));

        log.Verify(
            x => x.HandleLog(
                It.Is<TelegramContextFullLogMessage>(m =>
                    m.Request!.ChatId == 42
                    && m.Request.OperationType == "sendMessage"
                    && m.Request.MessageBody["Text"] == "hello"
                    ),
                It.IsAny<CancellationToken>()
                ),
            Times.Once
            );
    }

    [Test]
    public async Task SendRequest_WhenProtectContentDisabled_DoesNotTouchRequest()
    {
        var handler = new StubHttpMessageHandler(MessageResponseJson);
        var setting = new TBotSetting
        {
            BotName = "bot",
            Token = Token,
        };

        await using var context = CreateContext(handler, setting, CreateLogMock().Object);

        var request = new SendMessageRequest
        {
            ChatId = 42,
            Text = "hello",
        };

        await context.SendRequest(request, CancellationToken.None);

        Assert.That(request.ProtectContent, Is.False);
        Assert.That(handler.RequestBodies.Single(), Does.Not.Contain("protect_content"));
    }

    [Test]
    public void Constructor_WhenLocalServerAndTimeoutProvided_AppliesClientOptions()
    {
        var handler = new StubHttpMessageHandler(MessageResponseJson);
        var setting = new TBotSetting
        {
            BotName = "bot",
            Token = Token,
            BaseUrl = "http://localhost:8081",
            RequestTimeout = TimeSpan.FromSeconds(42),
        };

        using var httpClient = new HttpClient(handler);

        var context = CreateContext(httpClient, setting, CreateLogMock().Object);

        Assert.That(context.BotId, Is.EqualTo(123456));
        Assert.That(context.LocalBotServer, Is.True);
        Assert.That(context.Timeout, Is.EqualTo(TimeSpan.FromSeconds(42)));
    }

    [Test]
    public async Task DownloadFileData_WhenFileExists_ReturnsContentAndMetadata()
    {
        var payload = new byte[] { 1, 2, 3, 4 };
        var handler = new FileStubHttpMessageHandler(payload, GetFileResponseJson);

        await using var context = CreateContext(handler, new TBotSetting { BotName = "bot", Token = Token }, CreateLogMock().Object);

        var result = await context.DownloadFileData("AgAD", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Bytes, Is.EqualTo(payload));
        Assert.That(result.Value!.Name, Is.EqualTo("photos/file_1.jpg"));
        Assert.That(result.Value!.Size, Is.EqualTo(4));
    }

    [Test]
    public async Task DownloadFileData_WhenTelegramReturnsError_ReturnsFailureInsteadOfThrowing()
    {
        var handler = new StubHttpMessageHandler(
            """{"ok":false,"error_code":400,"description":"Bad Request: invalid file_id"}""",
            HttpStatusCode.BadRequest
            );

        await using var context = CreateContext(handler, new TBotSetting { BotName = "bot", Token = Token }, CreateLogMock().Object);

        var result = await context.DownloadFileData("AgAD", CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public async Task DisposeAsync_ReportsElapsedTimeOfAllRequests()
    {
        var handler = new StubHttpMessageHandler(MessageResponseJson);
        var log = CreateLogMock();

        var context = CreateContext(handler, new TBotSetting { BotName = "bot", Token = Token }, log.Object);

        await context.SendRequest(new SendMessageRequest { ChatId = 1, Text = "one" }, CancellationToken.None);
        await context.SendRequest(new SendMessageRequest { ChatId = 1, Text = "two" }, CancellationToken.None);

        await context.DisposeAsync();

        log.Verify(
            x => x.HandleEnqueueLog(2, It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Once
            );
    }

    private static Mock<ITelegramContextLog> CreateLogMock()
    {
        var log = new Mock<ITelegramContextLog>();

        log
            .Setup(x => x.HandleLog(It.IsAny<TelegramContextFullLogMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        log
            .Setup(x => x.HandleErrorLog(It.IsAny<TelegramContextFullLogMessage>(), It.IsAny<Exception>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        log
            .Setup(x => x.HandleEnqueueLog(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return log;
    }

    private static TelegramContext CreateContext(HttpMessageHandler handler, TBotSetting setting, ITelegramContextLog log)
        => CreateContext(new HttpClient(handler), setting, log);

    private static TelegramContext CreateContext(HttpClient httpClient, TBotSetting setting, ITelegramContextLog log)
        => new(httpClient, setting, log, new RecyclableMemoryStreamManager());

    /// <summary>
    /// Возвращает заранее заданный ответ на любой запрос и запоминает тело запроса.
    /// </summary>
    private sealed class StubHttpMessageHandler(string responseJson, HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
    {
        public List<string> RequestBodies { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content is not null)
            {
                RequestBodies.Add(await request.Content.ReadAsStringAsync(cancellationToken));
            }

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json"),
            };
        }
    }

    /// <summary>
    /// Отвечает метаданными на getFile и содержимым файла на скачивание.
    /// </summary>
    private sealed class FileStubHttpMessageHandler(byte[] fileBytes, string getFileResponseJson) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var isGetFileRequest = request.RequestUri!.AbsolutePath.EndsWith("/getFile", StringComparison.Ordinal);

            var response = isGetFileRequest
                ? new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(getFileResponseJson, Encoding.UTF8, "application/json"),
                }
                : new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(fileBytes),
                };

            return Task.FromResult(response);
        }
    }
}
