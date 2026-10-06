#nullable enable
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using TBotPlatform.Common.Builder;
using TBotPlatform.Contracts.Abstractions.Builder;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using TBotPlatform.Tests.Common.Builder.States;
using Telegram.Bot.Types;

namespace TBotPlatform.Tests.Common.Builder;

/// <summary>
/// Tests the bot builder: telegram context, receiving handler, state scanning
/// with validations, and the set of registered services.
/// </summary>
[TestFixture]
public class BotBuilderTests
{
    private const string BotName = "test-bot";

    private ServiceCollection _services = null!;
    private BotPlatformBuilder _platformBuilder = null!;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _platformBuilder = new BotPlatformBuilder(_services);
    }

    [Test]
    public void AddTelegramContext_WhenCalledTwice_ThrowsInvalidOperationException()
    {
        var bot = CreateBotBuilder();
        bot.AddTelegramContext();

        var exception = Assert.Throws<InvalidOperationException>(() => bot.AddTelegramContext());

        Assert.That(exception!.Message, Is.EqualTo("Контекст telegram ранее был добавлен."));
    }

    [Test]
    public void AddTelegramContext_WithCustomLogType_RegistersKeyedLogType()
    {
        var bot = CreateBotBuilder();
        bot.AddTelegramContext<BuilderTestTelegramContextLog>();
        AddStatesAndHandler(bot, withTelegramContext: false);

        bot.Build();

        var descriptor = _services.Single(z => z.ServiceType == typeof(ITelegramContextLog) && z.IsKeyedService);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(descriptor.ServiceKey, Is.EqualTo(BotName));
            Assert.That(descriptor.KeyedImplementationType, Is.EqualTo(typeof(BuilderTestTelegramContextLog)));
        }
    }

    [Test]
    public void AddTelegramContext_WithHttpClientConfiguration_RegistersKeyedTelegramContext()
    {
        var bot = CreateBotBuilder();
        bot.AddTelegramContext(client => client.Timeout = TimeSpan.FromSeconds(5));
        AddStatesAndHandler(bot, withTelegramContext: false);

        bot.Build();

        Assert.That(_services.Any(z => z.ServiceType == typeof(ITelegramContext) && z.IsKeyedService), Is.True);
    }

    [Test]
    public void AddStates_WhenAssemblyIsNull_ThrowsArgumentNullException()
    {
        var bot = CreateBotBuilder();

        Assert.Throws<ArgumentNullException>(() => bot.AddStates((Assembly)null!));
    }

    [Test]
    public void AddStates_WhenStateListIsNull_ThrowsArgumentNullException()
    {
        var bot = CreateBotBuilder();

        Assert.Throws<ArgumentNullException>(() => bot.AddStates((List<Type>)null!));
    }

    [Test]
    public void AddStates_WhenCalledTwice_AppendsSecondStateList()
    {
        var bot = CreateBotBuilder();
        bot.AddTelegramContext();
        bot.AddStates([typeof(StartBuilderTestState)]);
        bot.AddStates([typeof(NamedBuilderTestState)]);
        bot.AddReceivingHandler<BuilderTestReceivingHandler>();

        bot.Build();

        Assert.That(GetStateFactoryDataCollection().Count, Is.EqualTo(2));
    }

    [Test]
    public void AddReceivingHandler_WhenCalledTwice_ThrowsInvalidOperationException()
    {
        var bot = CreateBotBuilder();
        bot.AddReceivingHandler<BuilderTestReceivingHandler>();

        var exception = Assert.Throws<InvalidOperationException>(() => bot.AddReceivingHandler<BuilderTestReceivingHandler>());

        Assert.That(exception!.Message, Is.EqualTo("Обработчик событий от telegram ранее был добавлен."));
    }

    [Test]
    public void Build_WhenTelegramContextIsMissing_ThrowsInvalidOperationException()
    {
        var bot = CreateBotBuilder();
        AddStatesAndHandler(bot, withTelegramContext: false);

        var exception = Assert.Throws<InvalidOperationException>(() => bot.Build());

        Assert.That(exception!.Message, Is.EqualTo("Отсутствует контекст telegram."));
    }

    [Test]
    public void Build_WhenReceivingHandlerIsMissing_ThrowsInvalidOperationException()
    {
        var bot = CreateBotBuilder();
        bot.AddTelegramContext();
        bot.AddStates([typeof(StartBuilderTestState)]);

        var exception = Assert.Throws<InvalidOperationException>(() => bot.Build());

        Assert.That(exception!.Message, Is.EqualTo("Отсутствует обработчик событий от telegram."));
    }

    [Test]
    public void Build_WhenNoStateDefinesStartCommand_ThrowsException()
        => AssertBuildFails([typeof(NamedBuilderTestState)], BotName, "Нет состояний определяющих команду /start");

    [Test]
    public void Build_WhenMoreThanOneStateDefinesStartCommand_ThrowsException()
        => AssertBuildFails(
            [typeof(MultiStartBuilderStateOne), typeof(MultiStartBuilderStateTwo)],
            "multi-start-bot",
            "Состояний определяющих команду /start больше одного"
            );

    [Test]
    public void Build_WhenStateHasActivatorButDoesNotImplementIState_ThrowsException()
        => AssertBuildFails([typeof(AaaNotAStateBuilderTestState)], BotName, "не наследуется");

    [Test]
    public void Build_WhenStatesHaveDuplicatedButtonsTypes_ThrowsException()
        => AssertBuildFails(
            [typeof(ConflictButtonsBuilderStateOne), typeof(ConflictButtonsBuilderStateTwo)],
            "conflict-bot",
            "дубли по ButtonsTypes"
            );

    [Test]
    public void Build_WhenStatesHaveDuplicatedTextsTypes_ThrowsException()
        => AssertBuildFails(
            [typeof(ConflictTextsBuilderStateOne), typeof(ConflictTextsBuilderStateTwo)],
            "conflict-bot",
            "дубли по TextsTypes"
            );

    [Test]
    public void Build_WhenTwoStatesAreMarkedAsLockUser_ThrowsException()
        => AssertBuildFails(
            [typeof(LockBuilderStateOne), typeof(LockBuilderStateTwo), typeof(StartBuilderTestState)],
            "lock-bot",
            "IsLockUserState"
            );

    [Test]
    public void Build_WhenTwoStatesAreMarkedAsRegistration_ThrowsException()
        => AssertBuildFails(
            [typeof(RegistrationBuilderStateOne), typeof(RegistrationBuilderStateTwo), typeof(StartBuilderTestState)],
            "registration-bot",
            "IsRegistrationState"
            );

    [Test]
    public void Build_WhenMenuTypeDoesNotImplementMenuButton_ThrowsException()
        => AssertBuildFails([typeof(BadMenuBuilderTestState)], "bad-menu-bot", "не наследуется от IMenuButton");

    [Test]
    public void Build_WhenInlineStateAlsoDefinesMenuType_ThrowsException()
        => AssertBuildFails([typeof(InlineWithMenuBuilderTestState)], "inline-menu-bot", "IsInlineState");

    [Test]
    public void Build_WhenStatesHaveDuplicatedTypeNames_SkipsDuplicateInsteadOfRegistering()
    {
        var bot = CreateBotBuilder("duplicate-bot");
        bot.AddTelegramContext();
        bot.AddStates(
            [
                typeof(DuplicateNamedBuilderTestState),
                typeof(States.Duplicates.DuplicateNamedBuilderTestState),
                typeof(StartBuilderTestState),
            ]);
        bot.AddReceivingHandler<BuilderTestReceivingHandler>();

        bot.Build();

        Assert.That(GetStateFactoryDataCollection("duplicate-bot").Count, Is.EqualTo(2));
    }

    [Test]
    public void Build_WhenStateIsMarkedOnlyForAnotherBot_SkipsIt()
    {
        var bot = CreateBotBuilder();
        bot.AddTelegramContext();
        bot.AddStates([typeof(StartBuilderTestState), typeof(OnlyForOtherBotBuilderTestState)]);
        bot.AddReceivingHandler<BuilderTestReceivingHandler>();

        bot.Build();

        var states = GetStateFactoryDataCollection();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(states.Count, Is.EqualTo(1));
            Assert.That(states[0].StateTypeName, Is.EqualTo(nameof(StartBuilderTestState)));
        }
    }

    [Test]
    public void Build_WhenStateIsMarkedForCurrentBot_RegistersIt()
    {
        var bot = CreateBotBuilder("other-bot");
        bot.AddTelegramContext();
        bot.AddStates([typeof(StartBuilderTestState), typeof(OnlyForOtherBotBuilderTestState)]);
        bot.AddReceivingHandler<BuilderTestReceivingHandler>();

        bot.Build();

        Assert.That(GetStateFactoryDataCollection("other-bot").Count, Is.EqualTo(2));
    }

    [Test]
    public void Build_WhenStatesAreScannedFromAssemblyWithInvalidState_ThrowsException()
    {
        var bot = CreateBotBuilder();
        bot.AddTelegramContext();
        bot.AddStates(GetType().Assembly);
        bot.AddReceivingHandler<BuilderTestReceivingHandler>();

        var exception = Assert.Catch<Exception>(() => bot.Build());

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.Message, Does.Contain(nameof(AaaNotAStateBuilderTestState)));
        Assert.That(exception.Message, Does.Contain("не наследуется"));
    }

    [Test]
    public void Build_WhenScannedAssemblyHasNoActivatorTypes_ThrowsInvalidDataException()
    {
        var bot = CreateBotBuilder();
        bot.AddTelegramContext();
        bot.AddReceivingHandler<BuilderTestReceivingHandler>();

        var exception = Assert.Throws<InvalidDataException>(() => bot.AddStates(typeof(Uri).Assembly));

        Assert.That(exception!.Message, Is.EqualTo("Отсутствуют потенциальные состояния."));
    }

    [Test]
    public void Build_WithValidStates_ReturnsPlatformBuilder()
    {
        var bot = CreateBotBuilder();
        AddStatesAndHandler(bot);

        var result = bot.Build();

        Assert.That(result, Is.SameAs(_platformBuilder));
    }

    [Test]
    public void Build_WithValidStates_RegistersKeyedReceivingHandler()
    {
        var bot = CreateBotBuilder();
        AddStatesAndHandler(bot);

        bot.Build();

        var descriptor = _services.Single(z => z.ServiceType == typeof(IStartReceivingHandler) && z.IsKeyedService);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(descriptor.ServiceKey, Is.EqualTo(BotName));
            Assert.That(descriptor.KeyedImplementationType, Is.EqualTo(typeof(BuilderTestReceivingHandler)));
            Assert.That(descriptor.Lifetime, Is.EqualTo(ServiceLifetime.Scoped));
        }
    }

    [Test]
    public void Build_WithValidStates_RegistersKeyedStateFactoryDataCollection()
    {
        var bot = CreateBotBuilder();
        AddStatesAndHandler(bot);

        bot.Build();

        var states = GetStateFactoryDataCollection();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(states, Has.Count.EqualTo(3));
            Assert.That(states.Any(z => z.StateTypeName == nameof(StartBuilderTestState)), Is.True);
            Assert.That(states.Any(z => z.StateTypeName == nameof(MenuBuilderTestState)), Is.True);
        }
    }

    [Test]
    public void Build_WithValidStates_RegistersStateAndMenuTypes()
    {
        var bot = CreateBotBuilder();
        AddStatesAndHandler(bot);

        bot.Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_services.Any(z => z.ServiceType == typeof(StartBuilderTestState)), Is.True);
            Assert.That(_services.Any(z => z.ServiceType == typeof(BuilderTestMenuButton)), Is.True);
        }
    }

    [Test]
    public void Build_WithValidStates_RegistersMemoryStreamManager()
    {
        var bot = CreateBotBuilder();
        AddStatesAndHandler(bot);

        bot.Build();

        Assert.That(
            _services.Any(z => z.ServiceType == typeof(RecyclableMemoryStreamManager)),
            Is.True
            );
    }

    private IBotBuilder CreateBotBuilder(string botName = BotName)
        => _platformBuilder.AddBot(new TBotSetting { BotName = botName, Token = "123:token" });

    private static void AddStatesAndHandler(IBotBuilder bot, bool withTelegramContext = true)
    {
        if (withTelegramContext)
        {
            bot.AddTelegramContext();
        }

        bot.AddStates([typeof(StartBuilderTestState), typeof(NamedBuilderTestState), typeof(MenuBuilderTestState)]);
        bot.AddReceivingHandler<BuilderTestReceivingHandler>();
    }

    private void AssertBuildFails(IEnumerable<Type> stateTypes, string botName, string expectedMessagePart)
    {
        var bot = CreateBotBuilder(botName);
        bot.AddTelegramContext();
        bot.AddStates(stateTypes.ToList());
        bot.AddReceivingHandler<BuilderTestReceivingHandler>();

        var exception = Assert.Catch<Exception>(() => bot.Build());

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.Message, Does.Contain(expectedMessagePart));
    }

    private StateFactoryDataCollection GetStateFactoryDataCollection(string botName = BotName)
    {
        using var provider = _services.BuildServiceProvider();

        return provider.GetRequiredKeyedService<StateFactoryDataCollection>(botName);
    }

    private sealed class BuilderTestReceivingHandler : IStartReceivingHandler
    {
        public Task<IResult> HandleUpdate(
            string botName,
            Update update,
            MarkupNextState? markupNextState,
            TelegramMessageUserData telegramData,
            CancellationToken cancellationToken
            )
            => Task.FromResult<IResult>(Result.Success());
    }

    private sealed class BuilderTestTelegramContextLog : ITelegramContextLog
    {
        public Task HandleLog(TelegramContextFullLogMessage message, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task HandleErrorLog(TelegramContextFullLogMessage message, Exception exception, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task HandleEnqueueLog(int requestCount, int elapsedMilliseconds, Guid operationGuid, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
