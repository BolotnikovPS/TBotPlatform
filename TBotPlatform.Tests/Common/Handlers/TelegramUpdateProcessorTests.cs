#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Tests.Common.Handlers;

/// <summary>
/// Проверяет процессор обновлений: маршрутизацию в keyed-обработчик бота, разбор данных inline-кнопки,
/// пропуск неподдерживаемых обновлений и превращение любых сбоев в результат-провал.
/// </summary>
[TestFixture]
public class TelegramUpdateProcessorTests
{
    private const string BotName = "test-bot";

    private Mock<IStartReceivingHandler> _handler = null!;
    private ServiceProvider _provider = null!;
    private ITelegramUpdateProcessor _processor = null!;

    [SetUp]
    public void SetUp()
    {
        _handler = new Mock<IStartReceivingHandler>();

        _handler
            .Setup(x => x.HandleUpdate(It.IsAny<string>(), It.IsAny<Update>(), It.IsAny<MarkupNextState?>(), It.IsAny<TelegramMessageUserData>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IResult>(Result.Success()));

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddKeyedSingleton<ITelegramContext>(BotName, Mock.Of<ITelegramContext>());
        services.AddKeyedScoped<IStartReceivingHandler>(BotName, (_, _) => _handler.Object);

        _provider = services.BuildServiceProvider();
        _processor = new TelegramUpdateProcessor(NullLogger<TelegramUpdateProcessor>.Instance, _provider);
    }

    [TearDown]
    public void TearDown() => _provider.Dispose();

    [Test]
    public void ProcessUpdates_WhenUpdatesAreEmpty_DoesNotResolveAnythingFromContainer()
    {
        var services = new Mock<IServiceProvider>();
        var processor = new TelegramUpdateProcessor(NullLogger<TelegramUpdateProcessor>.Instance, services.Object);

        Assert.DoesNotThrowAsync(() => processor.ProcessUpdates(BotName, [], CancellationToken.None));

        services.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ProcessUpdate_WhenMessageUpdate_InvokesKeyedHandlerOnceWithParsedUserData()
    {
        var update = CreateMessageUpdate(id: 1, chatId: 100);
        TelegramMessageUserData? capturedUserData = null;

        _handler
            .Setup(x => x.HandleUpdate(BotName, update, It.Is<MarkupNextState?>(markup => markup == null), It.IsAny<TelegramMessageUserData>(), CancellationToken.None))
            .Callback<string, Update, MarkupNextState?, TelegramMessageUserData, CancellationToken>((_, _, _, data, _) => capturedUserData = data)
            .Returns(Task.FromResult<IResult>(Result.Success()));

        var result = await _processor.ProcessUpdate(BotName, update, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(capturedUserData, Is.Not.Null);
            Assert.That(capturedUserData!.ChatOrNull?.Id, Is.EqualTo(100));
            Assert.That(capturedUserData.UserOrNull?.Id, Is.EqualTo(42));
        }

        _handler.Verify(
            x => x.HandleUpdate(BotName, update, It.Is<MarkupNextState?>(markup => markup == null), It.IsAny<TelegramMessageUserData>(), CancellationToken.None),
            Times.Once
            );
    }

    [Test]
    public async Task ProcessUpdate_WhenCallbackDataIsJson_PassesParsedMarkupNextStateToHandler()
    {
        var update = CreateCallbackUpdate(callbackId: "cb-1", data: JsonConvert.SerializeObject(new MarkupNextState("TestState", "42")));
        MarkupNextState? capturedMarkup = null;

        _handler
            .Setup(x => x.HandleUpdate(BotName, update, It.IsAny<MarkupNextState?>(), It.IsAny<TelegramMessageUserData>(), CancellationToken.None))
            .Callback<string, Update, MarkupNextState?, TelegramMessageUserData, CancellationToken>((_, _, markup, _, _) => capturedMarkup = markup)
            .Returns(Task.FromResult<IResult>(Result.Success()));

        var result = await _processor.ProcessUpdate(BotName, update, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(capturedMarkup, Is.Not.Null);
            Assert.That(capturedMarkup!.State, Is.EqualTo("TestState"));
            Assert.That(capturedMarkup.Data, Is.EqualTo("42"));
        }
    }

    [Test]
    public async Task ProcessUpdate_WhenUpdateTypeIsNotSupportedByPlatform_ReturnsSuccessWithoutInvokingHandler()
    {
        var update = new Update { Id = 1 };

        var result = await _processor.ProcessUpdate(BotName, update, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(update.Type, Is.EqualTo(UpdateType.Unknown));
            Assert.That(result.IsSuccess, Is.True);
        }

        VerifyHandlerWasNotCalled();
    }

    [Test]
    public async Task ProcessUpdate_WhenUpdateHasNoChat_ReturnsSuccessWithoutInvokingHandler()
    {
        var update = new Update
        {
            Id = 2,
            ManagedBot = new ManagedBotUpdated
            {
                User = new User { Id = 42, IsBot = false, FirstName = "user" },
                Bot = new User { Id = 43, IsBot = true, FirstName = "bot" },
            },
        };

        var result = await _processor.ProcessUpdate(BotName, update, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);

        VerifyHandlerWasNotCalled();
    }

    [Test]
    public async Task ProcessUpdate_WhenHandlerReturnsFailure_ReturnsFailureInsteadOfSuccess()
    {
        var update = CreateMessageUpdate(id: 3, chatId: 100);

        _handler
            .Setup(x => x.HandleUpdate(It.IsAny<string>(), It.IsAny<Update>(), It.IsAny<MarkupNextState?>(), It.IsAny<TelegramMessageUserData>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IResult>(Result.Failure(ErrorResult.Failure("x"))));

        var result = await _processor.ProcessUpdate(BotName, update, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error?.Description, Is.EqualTo("x"));
        }
    }

    [Test]
    public async Task ProcessUpdate_WhenHandlerThrows_ReturnsFailureInsteadOfThrowing()
    {
        var update = CreateMessageUpdate(id: 4, chatId: 100);

        _handler
            .Setup(x => x.HandleUpdate(It.IsAny<string>(), It.IsAny<Update>(), It.IsAny<MarkupNextState?>(), It.IsAny<TelegramMessageUserData>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        IResult? result = null;

        Assert.DoesNotThrowAsync(async () => result = await _processor.ProcessUpdate(BotName, update, CancellationToken.None));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.False);
            Assert.That(result.Error?.Description, Is.EqualTo("boom"));
        }
    }

    [Test]
    public async Task ProcessUpdates_WhenTwoUpdatesFromDifferentChats_InvokesHandlerForBoth()
    {
        var first = CreateMessageUpdate(id: 5, chatId: 100);
        var second = CreateMessageUpdate(id: 6, chatId: 200);

        await _processor.ProcessUpdates(BotName, [first, second], CancellationToken.None);

        _handler.Verify(
            x => x.HandleUpdate(BotName, It.IsAny<Update>(), It.IsAny<MarkupNextState?>(), It.IsAny<TelegramMessageUserData>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
            );
    }

    private void VerifyHandlerWasNotCalled()
        => _handler.Verify(
            x => x.HandleUpdate(It.IsAny<string>(), It.IsAny<Update>(), It.IsAny<MarkupNextState?>(), It.IsAny<TelegramMessageUserData>(), It.IsAny<CancellationToken>()),
            Times.Never
            );

    private static Update CreateMessageUpdate(int id, long chatId) => new()
    {
        Id = id,
        Message = new Message
        {
            Id = id,
            From = new User { Id = 42, IsBot = false, FirstName = "user" },
            Chat = new Chat { Id = chatId, Type = ChatType.Private },
            Text = "hello",
        },
    };

    private static Update CreateCallbackUpdate(string callbackId, string? data) => new()
    {
        Id = 10,
        CallbackQuery = new CallbackQuery
        {
            Id = callbackId,
            Data = data,
            From = new User { Id = 42, IsBot = false, FirstName = "user" },
            Message = new Message
            {
                Id = 11,
                Chat = new Chat { Id = 100, Type = ChatType.Private },
            },
        },
    };
}
