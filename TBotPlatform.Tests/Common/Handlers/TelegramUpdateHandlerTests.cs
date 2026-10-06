#nullable enable
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TBotPlatform.Tests.Common.Handlers;

/// <summary>
/// Tests the update handler through which ReceiveAsync from Telegram.Bot passes updates to the platform.
/// </summary>
[TestFixture]
public class TelegramUpdateHandlerTests
{
    [Test]
    public async Task HandleUpdateAsync_WhenUpdateReceived_PassesItToProcessor()
    {
        var processor = new Mock<ITelegramUpdateProcessor>();

        processor
            .Setup(x => x.ProcessUpdate("bot", It.IsAny<Update>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IResult>(Result.Success()));

        var handler = new TelegramUpdateHandler(processor.Object, "bot", Mock.Of<ILogger>());
        var update = new Update { Id = 7 };

        await handler.HandleUpdateAsync(Mock.Of<ITelegramBotClient>(), update, CancellationToken.None);

        processor.Verify(x => x.ProcessUpdate("bot", update, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestCase(400)]
    [TestCase(403)]
    [TestCase(409)]
    [TestCase(429)]
    [TestCase(500)]
    public void HandleErrorAsync_WhenApiExceptionReceived_DoesNotThrow(int errorCode)
    {
        var handler = new TelegramUpdateHandler(Mock.Of<ITelegramUpdateProcessor>(), "bot", Mock.Of<ILogger>());

        var exception = new ApiRequestException(
            "error",
            errorCode,
            new ResponseParameters { RetryAfter = 3, MigrateToChatId = 555 }
            );

        Assert.DoesNotThrowAsync(
            () => handler.HandleErrorAsync(Mock.Of<ITelegramBotClient>(), exception, HandleErrorSource.PollingError, CancellationToken.None)
            );
    }

    [Test]
    public void HandleErrorAsync_WhenUnexpectedExceptionReceived_DoesNotThrow()
    {
        var handler = new TelegramUpdateHandler(Mock.Of<ITelegramUpdateProcessor>(), "bot", Mock.Of<ILogger>());

        Assert.DoesNotThrowAsync(
            () => handler.HandleErrorAsync(
                Mock.Of<ITelegramBotClient>(),
                new InvalidOperationException("boom"),
                HandleErrorSource.HandleUpdateError,
                CancellationToken.None
                )
            );
    }

    [Test]
    public void HandleErrorAsync_WhenServiceStopped_DoesNotThrow()
    {
        var handler = new TelegramUpdateHandler(Mock.Of<ITelegramUpdateProcessor>(), "bot", Mock.Of<ILogger>());

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.DoesNotThrowAsync(
            () => handler.HandleErrorAsync(
                Mock.Of<ITelegramBotClient>(),
                new OperationCanceledException(),
                HandleErrorSource.PollingError,
                cts.Token
                )
            );
    }
}
