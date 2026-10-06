#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IO;
using Moq;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Queues;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.Users;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Tests.Common.Factories;

/// <summary>
/// Tests the state context factory: argument validation, selecting the state from DI,
/// scope ownership (released on error), and state-handling scenarios.
/// </summary>
[TestFixture]
public class StateContextFactoryTests
{
    private const string BotName = "test";
    private const long ChatIdValue = 777;
    private const string ErrorText = "err";
    private const string CallbackId = "callback-id";

    private Mock<ITelegramContext> _telegramContext = null!;
    private bool _telegramContextDisposed;
    private ServiceProvider _provider = null!;
    private StateContextFactory _factory = null!;

    [SetUp]
    public void SetUp()
    {
        _telegramContextDisposed = false;
        _telegramContext = new Mock<ITelegramContext>();

        _telegramContext
            .Setup(x => x.GetBotSetting())
            .Returns(new TBotSetting
            {
                BotName = BotName,
                Token = "1:token",
                ParseMode = ParseMode.Html,
                StateErrorText = ErrorText,
            });

        _telegramContext
            .Setup(x => x.CurrentOperation)
            .Returns(Guid.NewGuid());

        _telegramContext
            .As<IAsyncDisposable>()
            .Setup(x => x.DisposeAsync())
            .Callback(() => _telegramContextDisposed = true)
            .Returns(ValueTask.CompletedTask);
    }

    [TearDown]
    public void TearDown() => _provider?.Dispose();

    [Test]
    public void GetStateContext_WhenChatIdIsZero_Throws()
    {
        CreateFactory();

        Assert.Throws<ChatIdArgException>(() => _factory.GetStateContext(BotName, 0));
    }

    [Test]
    public void GetStateContext_WhenBotNameIsNull_Throws()
    {
        CreateFactory();

        Assert.Throws<ArgumentNullException>(() => _factory.GetStateContext(null!, ChatIdValue));
    }

    [Test]
    public async Task GetStateContext_WhenArgumentsAreValid_ReturnsContextOwningItsScope()
    {
        CreateFactory();

        var stateContext = _factory.GetStateContext(BotName, ChatIdValue);

        Assert.That(stateContext, Is.Not.Null);
        Assert.That(stateContext.TelegramContext, Is.SameAs(_telegramContext.Object));

        await stateContext.DisposeAsync();

        Assert.That(_telegramContextDisposed, Is.True, "scope контекста должен освобождать keyed ITelegramContext");
    }

    [Test]
    public void CreateStateContext_WhenUserIsNull_Throws()
    {
        CreateFactory();

        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _factory.CreateStateContext<TestUser>(BotName, null!, new StateHistory(typeof(TestState)), CreateTextUpdate(), CancellationToken.None));
    }

    [Test]
    public void CreateStateContext_WhenStateHistoryIsNull_Throws()
    {
        CreateFactory();

        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _factory.CreateStateContext(BotName, CreateUser(), null!, CreateTextUpdate(), CancellationToken.None));
    }

    [Test]
    public void CreateStateContext_WhenChatUpdateIsNull_Throws()
    {
        CreateFactory();

        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _factory.CreateStateContext(BotName, CreateUser(), new StateHistory(typeof(TestState)), null!, CancellationToken.None));
    }

    [Test]
    public void CreateStateContext_WhenStateTypeIsNotState_ThrowsAndReleasesScope()
    {
        CreateFactory();

        var exception = Assert.CatchAsync<Exception>(() =>
            _factory.CreateStateContext(BotName, CreateUser(), new StateHistory(typeof(string)), CreateTextUpdate(), CancellationToken.None));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception!.Message, Does.Contain("не наследуется"));
            Assert.That(_telegramContextDisposed, Is.True, "при ошибке после создания scope фабрика обязана его освободить");
        }
    }

    [Test]
    public void CreateStateContext_WhenAdminStateRequestedBySimpleUser_Throws()
    {
        CreateFactory();

        var user = CreateUser(isAdmin: false);
        var stateHistory = new StateHistory(typeof(TestState), isAdminState: true);

        var exception = Assert.CatchAsync<Exception>(() =>
            _factory.CreateStateContext(BotName, user, stateHistory, CreateTextUpdate(), CancellationToken.None));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception!.Message, Is.EqualTo("Пользователь не является администратором."));

            // The administrator check runs before the scope is created, so there is nothing to release:
            // the container never received a request for the keyed ITelegramContext.
            Assert.That(_telegramContextDisposed, Is.False);
        }
    }

    [Test]
    public async Task CreateStateContext_WhenStateSucceeds_SendsTypingAndCallsHandleAndHandleComplete()
    {
        var state = new TestState();
        CreateFactory(services => services.AddScoped(_ => state));

        var chatActionRequests = new List<SendChatActionRequest>();

        _telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SendChatActionRequest>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<bool>, CancellationToken>((request, _) => chatActionRequests.Add((SendChatActionRequest)request))
            .ReturnsAsync(true);

        var user = CreateUser();

        await using var stateContext = await _factory.CreateStateContext(BotName, user, new StateHistory(typeof(TestState)), CreateTextUpdate(), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(stateContext, Is.Not.Null);
            Assert.That(chatActionRequests, Has.Count.EqualTo(1));
            Assert.That(chatActionRequests[0].Action, Is.EqualTo(ChatAction.Typing));
            Assert.That(chatActionRequests[0].ChatId.Identifier, Is.EqualTo(ChatIdValue));
            Assert.That(state.HandleCalls, Is.EqualTo(1));
            Assert.That(state.HandleCompleteCalls, Is.EqualTo(1));
            Assert.That(state.HandleErrorCalls, Is.Zero);
            Assert.That(state.LastUser, Is.SameAs(user));
        }
    }

    [Test]
    public async Task CreateStateContext_WhenHandleThrows_SendsErrorTextAndCallsHandleErrorWithoutThrowing()
    {
        var handleException = new InvalidOperationException("state boom");
        var state = new TestState { HandleException = handleException };
        CreateFactory(services => services.AddScoped(_ => state));

        SetupChatAction();
        var messages = SetupSendMessageCapture();

        IStateContextMinimal? stateContext = null;

        Assert.DoesNotThrowAsync(async () =>
            stateContext = await _factory.CreateStateContext(BotName, CreateUser(), new StateHistory(typeof(TestState)), CreateTextUpdate(), CancellationToken.None));

        await using (stateContext)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(stateContext, Is.Not.Null);
                Assert.That(messages, Has.Count.EqualTo(1));
                Assert.That(messages[0].Text, Is.EqualTo(ErrorText));
                Assert.That(messages[0].ChatId.Identifier, Is.EqualTo(ChatIdValue));
                Assert.That(state.HandleErrorCalls, Is.EqualTo(1));
                Assert.That(state.LastError, Is.SameAs(handleException));
                Assert.That(state.HandleCompleteCalls, Is.Zero);
            }
        }
    }

    [Test]
    public async Task CreateStateContext_WhenUpdateIsCallbackQuery_AnswersCallbackQuery()
    {
        var state = new TestState();
        CreateFactory(services => services.AddScoped(_ => state));

        SetupChatAction();

        var callbackRequests = new List<AnswerCallbackQueryRequest>();

        _telegramContext
            .Setup(x => x.SendRequest(It.IsAny<AnswerCallbackQueryRequest>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<bool>, CancellationToken>((request, _) => callbackRequests.Add((AnswerCallbackQueryRequest)request))
            .ReturnsAsync(true);

        await using var stateContext = await _factory.CreateStateContext(
            BotName,
            CreateUser(),
            new StateHistory(typeof(TestState)),
            CreateCallbackUpdate(),
            CancellationToken.None
            );

        using (Assert.EnterMultipleScope())
        {
            Assert.That(stateContext, Is.Not.Null);
            Assert.That(callbackRequests, Has.Count.EqualTo(1));
            Assert.That(callbackRequests[0].CallbackQueryId, Is.EqualTo(CallbackId));
        }
    }

    [Test]
    public async Task CreateStateContext_WhenSendChatActionThrows_StillHandlesState()
    {
        var state = new TestState();
        CreateFactory(services => services.AddScoped(_ => state));

        _telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SendChatActionRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("typing failed"));

        await using var stateContext = await _factory.CreateStateContext(BotName, CreateUser(), new StateHistory(typeof(TestState)), CreateTextUpdate(), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(stateContext, Is.Not.Null);
            Assert.That(state.HandleCalls, Is.EqualTo(1));
            Assert.That(state.HandleCompleteCalls, Is.EqualTo(1));
            Assert.That(state.HandleErrorCalls, Is.Zero);
        }
    }

    private void CreateFactory(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddKeyedScoped<ITelegramContext>(BotName, (_, _) => _telegramContext.Object);
        services.AddSingleton(Mock.Of<IStateBindFactory>());
        services.AddSingleton(Mock.Of<IDelayQueue>());
        services.AddSingleton(new RecyclableMemoryStreamManager());

        configure?.Invoke(services);

        _provider = services.BuildServiceProvider();
        _factory = new StateContextFactory(NullLogger<StateContextFactory>.Instance, _provider.GetRequiredService<IServiceScopeFactory>());
    }

    private void SetupChatAction()
        => _telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SendChatActionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

    private List<SendMessageRequest> SetupSendMessageCapture()
    {
        var captured = new List<SendMessageRequest>();

        _telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((request, _) => captured.Add((SendMessageRequest)request))
            .ReturnsAsync(new Message());

        return captured;
    }

    private static TestUser CreateUser(bool isAdmin = false) => new()
    {
        ChatId = ChatIdValue,
        TgUserId = 42,
        UserName = "user",
        IsAdminValue = isAdmin,
    };

    private static Update CreateTextUpdate() => new()
    {
        Id = 1,
        Message = new Message
        {
            Id = 1,
            From = new User { Id = 42, IsBot = false, FirstName = "user" },
            Chat = new Chat { Id = ChatIdValue, Type = ChatType.Private },
            Text = "hello",
        },
    };

    private static Update CreateCallbackUpdate() => new()
    {
        Id = 2,
        CallbackQuery = new CallbackQuery
        {
            Id = CallbackId,
            From = new User { Id = 42, IsBot = false, FirstName = "user" },
            Message = new Message
            {
                Id = 5,
                Chat = new Chat { Id = ChatIdValue, Type = ChatType.Private },
            },
        },
    };

    private sealed class TestUser : UserBase
    {
        public bool IsAdminValue { get; init; }

        public override bool IsAdmin() => IsAdminValue;
    }

    private sealed class TestState : IState<TestUser>
    {
        public int HandleCalls { get; private set; }

        public int HandleCompleteCalls { get; private set; }

        public int HandleErrorCalls { get; private set; }

        public Exception? HandleException { get; init; }

        public Exception? LastError { get; private set; }

        public TestUser? LastUser { get; private set; }

        public Task Handle(IStateContext context, TestUser user, CancellationToken cancellationToken)
        {
            HandleCalls++;
            LastUser = user;

            return HandleException is null
                ? Task.CompletedTask
                : Task.FromException(HandleException);
        }

        public Task HandleComplete(IStateContext context, TestUser user, CancellationToken cancellationToken)
        {
            HandleCompleteCalls++;

            return Task.CompletedTask;
        }

        public Task HandleError(IStateContext context, TestUser user, Exception exception, CancellationToken cancellationToken)
        {
            HandleErrorCalls++;
            LastError = exception;

            return Task.CompletedTask;
        }
    }
}
