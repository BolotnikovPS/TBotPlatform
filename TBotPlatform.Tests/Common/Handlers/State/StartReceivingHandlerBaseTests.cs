#nullable enable
using Moq;
using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Results;
using TBotPlatform.Results.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Tests.Common.Handlers.State;

/// <summary>
/// Tests the base state handler: update routing (inline data, pinned state,
/// command, button text, last menu), hook call order, and missing-chat handling.
/// </summary>
[TestFixture]
public class StartReceivingHandlerBaseTests
{
    private const string BotName = "test";
    private const long ChatIdValue = 555;

    private Mock<IStateFactory> _stateFactory = null!;
    private Mock<IStateContextFactory> _stateContextFactory = null!;
    private TestHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _stateFactory = new Mock<IStateFactory>();
        _stateContextFactory = new Mock<IStateContextFactory>();

        // By default the state is not pinned.
        _stateFactory
            .Setup(x => x.HasBindState(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultT<bool>.Success(false));

        _stateContextFactory
            .Setup(x => x.CreateStateContext(
                It.IsAny<string>(),
                It.IsAny<TestUser>(),
                It.IsAny<StateHistory>(),
                It.IsAny<Update>(),
                It.IsAny<MarkupNextState?>(),
                It.IsAny<CancellationToken>()
                ))
            .ReturnsAsync(Mock.Of<IStateContextMinimal>());

        _handler = new TestHandler(_stateFactory.Object, _stateContextFactory.Object);
    }

    [Test]
    public async Task HandleUpdate_WhenChatIsMissing_ReturnsFailureAndDoesNotTouchFactories()
    {
        var result = await _handler.HandleUpdate(
            BotName,
            CreateTextUpdate("hello"),
            null,
            new TelegramMessageUserData(CreateTelegramUser(), null),
            CancellationToken.None
            );

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.ErrorType, Is.EqualTo(ErrorResultType.NotFound));
            Assert.That(_handler.GetOrCreateUserCalls, Is.Zero);
            VerifyStateContextCreation(Times.Never(), null);
        }
    }

    [Test]
    public async Task HandleUpdate_WhenMarkupNextStateHasState_UsesStateByNameAndRunsHooks()
    {
        var stateHistory = new StateHistory(typeof(TestState));
        var update = CreateTextUpdate("hello");

        _stateFactory
            .Setup(x => x.GetStateByNameOrDefault(BotName, "MainMenu"))
            .Returns(ResultT<StateHistory>.Success(stateHistory));

        var result = await _handler.HandleUpdate(BotName, update, new MarkupNextState("MainMenu"), CreateUserData(), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(_handler.GetOrCreateUserCalls, Is.EqualTo(1));
            Assert.That(_handler.BeforeHandleCalls, Is.EqualTo(1));
            Assert.That(_handler.AfterHandleCalls, Is.EqualTo(1));
            VerifyStateContextCreation(Times.Once(), stateHistory);
        }
    }

    [Test]
    public async Task HandleUpdate_WhenStateIsNotFound_ReturnsFailureWithoutStateContext()
    {
        _stateFactory
            .Setup(x => x.GetStateByNameOrDefault(BotName, "MainMenu"))
            .Returns(ResultT<StateHistory>.Failure(ErrorResult.NotFound("Состояние не найдено.")));

        var result = await _handler.HandleUpdate(BotName, CreateTextUpdate("hello"), new MarkupNextState("MainMenu"), CreateUserData(), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.ErrorType, Is.EqualTo(ErrorResultType.NotFound));
            Assert.That(_handler.BeforeHandleCalls, Is.EqualTo(1));
            Assert.That(_handler.AfterHandleCalls, Is.Zero, "хук после обработки не вызывается, если состояние не найдено");
            VerifyStateContextCreation(Times.Never(), null);
        }
    }

    [Test]
    public async Task HandleUpdate_WhenBindStateExists_UsesBoundState()
    {
        var stateHistory = new StateHistory(typeof(TestState));

        _stateFactory
            .Setup(x => x.HasBindState(BotName, ChatIdValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultT<bool>.Success(true));

        _stateFactory
            .Setup(x => x.GetBindStateOrNull(BotName, ChatIdValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultT<StateHistory>.Success(stateHistory));

        var result = await _handler.HandleUpdate(BotName, CreateTextUpdate("hello"), null, CreateUserData(), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            VerifyStateContextCreation(Times.Once(), stateHistory);
        }
    }

    [Test]
    public async Task HandleUpdate_WhenTextIsCommandWithBotName_ResolvesCommandWithoutMention()
    {
        var stateHistory = new StateHistory(typeof(TestState));

        _stateFactory
            .Setup(x => x.GetStateByCommandsTypeOrDefault(BotName, ChatIdValue, "/start", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultT<StateHistory>.Success(stateHistory));

        var result = await _handler.HandleUpdate(BotName, CreateTextUpdate("/start@test_bot"), null, CreateUserData(), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            VerifyStateContextCreation(Times.Once(), stateHistory);
        }
    }

    [Test]
    public async Task HandleUpdate_WhenTextIsButton_ResolvesStateByButtonsType()
    {
        var stateHistory = new StateHistory(typeof(TestState));

        _stateFactory
            .Setup(x => x.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, "Инфо", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultT<StateHistory>.Success(stateHistory));

        var result = await _handler.HandleUpdate(BotName, CreateTextUpdate("Инфо"), null, CreateUserData(), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            VerifyStateContextCreation(Times.Once(), stateHistory);
        }
    }

    [Test]
    public async Task HandleUpdate_WhenUpdateHasNoText_FallsBackToLastStateWithMenu()
    {
        var stateHistory = new StateHistory(typeof(TestState));

        _stateFactory
            .Setup(x => x.GetLastStateWithMenu(BotName, ChatIdValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultT<StateHistory>.Success(stateHistory));

        var result = await _handler.HandleUpdate(BotName, CreatePhotoUpdate(), null, CreateUserData(), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            VerifyStateContextCreation(Times.Once(), stateHistory);
        }
    }

    private void VerifyStateContextCreation(Times times, StateHistory? expectedHistory)
    {
        // The branching is kept out of the Moq expression: pattern matching is not supported in expression trees.
        if (expectedHistory is null)
        {
            _stateContextFactory.Verify(
                x => x.CreateStateContext(
                    BotName,
                    It.Is<TestUser>(user => user.ChatId == ChatIdValue),
                    It.IsAny<StateHistory>(),
                    It.IsAny<Update>(),
                    It.IsAny<MarkupNextState?>(),
                    It.IsAny<CancellationToken>()
                    ),
                times
                );

            return;
        }

        _stateContextFactory.Verify(
            x => x.CreateStateContext(
                BotName,
                It.Is<TestUser>(user => user.ChatId == ChatIdValue),
                It.Is<StateHistory>(history => history == expectedHistory),
                It.IsAny<Update>(),
                It.IsAny<MarkupNextState?>(),
                It.IsAny<CancellationToken>()
                ),
            times
            );
    }

    private static TelegramMessageUserData CreateUserData()
        => new(CreateTelegramUser(), new Chat { Id = ChatIdValue, Type = ChatType.Private });

    private static User CreateTelegramUser() => new()
    {
        Id = 42,
        IsBot = false,
        FirstName = "user",
    };

    private static TestUser CreateUser() => new()
    {
        ChatId = ChatIdValue,
        TgUserId = 42,
        UserName = "user",
    };

    private static Update CreateTextUpdate(string text) => new()
    {
        Id = 1,
        Message = new Message
        {
            Id = 1,
            From = new User { Id = 42, IsBot = false, FirstName = "user" },
            Chat = new Chat { Id = ChatIdValue, Type = ChatType.Private },
            Text = text,
        },
    };

    private static Update CreatePhotoUpdate() => new()
    {
        Id = 2,
        Message = new Message
        {
            Id = 2,
            From = new User { Id = 42, IsBot = false, FirstName = "user" },
            Chat = new Chat { Id = ChatIdValue, Type = ChatType.Private },
            Photo =
            [
                new PhotoSize
                {
                    FileId = "file-id",
                    FileUniqueId = "file-unique-id",
                    Width = 10,
                    Height = 10,
                },
            ],
        },
    };

    private sealed class TestUser : UserBase
    {
        public override bool IsAdmin() => false;
    }

    private sealed class TestState : IState<TestUser>
    {
        public Task Handle(IStateContext context, TestUser user, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task HandleComplete(IStateContext context, TestUser user, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task HandleError(IStateContext context, TestUser user, Exception exception, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class TestHandler(IStateFactory stateFactory, IStateContextFactory stateContextFactory)
        : StartReceivingHandlerBase<TestUser>(stateFactory, stateContextFactory)
    {
        public int GetOrCreateUserCalls { get; private set; }

        public int BeforeHandleCalls { get; private set; }

        public int AfterHandleCalls { get; private set; }

        protected override Task<TestUser> GetOrCreateUser(string botName, TelegramMessageUserData telegramData, CancellationToken cancellationToken)
        {
            GetOrCreateUserCalls++;

            return Task.FromResult(CreateUser());
        }

        protected override Task BeforeHandleAsync(string botName, Update update, TestUser user, CancellationToken cancellationToken)
        {
            BeforeHandleCalls++;

            return Task.CompletedTask;
        }

        protected override Task AfterHandleAsync(string botName, Update update, TestUser user, CancellationToken cancellationToken)
        {
            AfterHandleCalls++;

            return Task.CompletedTask;
        }
    }
}
