using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Contracts.Cache;
using TBotPlatform.Results;

namespace TBotPlatform.Tests.Common.Factories;

[TestFixture]
public class StateFactoryTests
{
    private Mock<ICacheService> _cache = null!;
    private ServiceProvider _serviceProvider = null!;
    private Assembly _assembly = null!;
    private StateFactory _factory = null!;

    [SetUp]
    public void SetUp()
    {
        _cache = new Mock<ICacheService>();
        _assembly = typeof(Result).Assembly;
    }

    private void CreateFactory(string botName, IList<StateFactoryData> stateDataList)
    {
        var collection = new StateFactoryDataCollection(stateDataList);
        var services = new ServiceCollection();
        services.AddKeyedSingleton(botName, collection);
        _serviceProvider = services.BuildServiceProvider();
        _factory = new StateFactory(_cache.Object, _serviceProvider, _assembly);
    }

    [Test]
    public void HasState_WhenCollectionEmpty_ReturnsFalse()
    {
        CreateFactory("bot1", []);
        Assert.That(_factory.HasState("bot1", "SomeState"), Is.False);
    }

    [Test]
    public void HasState_WhenStateExists_ReturnsTrue()
    {
        var data = new StateFactoryData("TestState");
        CreateFactory("bot1", [data]);
        Assert.That(_factory.HasState("bot1", "TestState"), Is.True);
    }

    [Test]
    public void HasState_WhenStateDoesNotExist_ReturnsFalse()
    {
        var data = new StateFactoryData("TestState");
        CreateFactory("bot1", [data]);
        Assert.That(_factory.HasState("bot1", "OtherState"), Is.False);
    }

    [Test]
    public void HasStates_WhenAnyStateExists_ReturnsTrue()
    {
        var data = new StateFactoryData("A");
        CreateFactory("bot1", [data]);
        Assert.That(_factory.HasStates("bot1", ["X", "A"]), Is.True);
    }

    [Test]
    public void HasStates_WhenNoneExist_ReturnsFalse()
    {
        var data = new StateFactoryData("A");
        CreateFactory("bot1", [data]);
        Assert.That(_factory.HasStates("bot1", ["X", "Y"]), Is.False);
    }

    [Test]
    public void GetStateByNameOrDefault_WhenNameEmpty_ReturnsStartStateIfExists()
    {
        var startState = new StateFactoryData("Result", commandsTypes: [CommandTypesConstant.StartCommand]);
        CreateFactory("bot1", [startState]);

        var result = _factory.GetStateByNameOrDefault("bot1", "");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.StateType.Name, Is.EqualTo("Result"));
        }
    }

    [Test]
    public void GetStateByNameOrDefault_WhenStateExists_ReturnsState()
    {
        var data = new StateFactoryData("Result");
        CreateFactory("bot1", [data]);

        var result = _factory.GetStateByNameOrDefault("bot1", "Result");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.StateType.Name, Is.EqualTo("Result"));
        }
    }

    [Test]
    public void GetStateByNameOrDefault_WhenStateNotFound_ReturnsFailure()
    {
        CreateFactory("bot1", []);

        var result = _factory.GetStateByNameOrDefault("bot1", "NonExistentTypeName12345");

        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void GetLockState_WhenLockStateExists_ReturnsState()
    {
        var lockState = new StateFactoryData("Result", isLockState: true);
        CreateFactory("bot1", [lockState]);

        var result = _factory.GetLockState("bot1");

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void GetLockState_WhenNoLockState_ReturnsFailure()
    {
        var data = new StateFactoryData("Result", isLockState: false);
        CreateFactory("bot1", [data]);

        var result = _factory.GetLockState("bot1");

        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void GetRegistrationState_WhenRegistrationStateExists_ReturnsState()
    {
        var regState = new StateFactoryData("Result", isRegistrationState: true);
        CreateFactory("bot1", [regState]);

        var result = _factory.GetRegistrationState("bot1");

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void GetRegistrationState_WhenNoRegistrationState_ReturnsFailure()
    {
        CreateFactory("bot1", []);

        var result = _factory.GetRegistrationState("bot1");

        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public void GetStateByTextsTypeOrDefault_WhenMatchExists_ReturnsState()
    {
        var data = new StateFactoryData("Result", textsTypes: ["hello"]);
        CreateFactory("bot1", [data]);

        var result = _factory.GetStateByTextsTypeOrDefault("bot1", 123L, "hello");

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void GetStateByTextsTypeOrDefault_WhenNoMatch_ReturnsStartState()
    {
        var startState = new StateFactoryData("Result", commandsTypes: [CommandTypesConstant.StartCommand]);
        CreateFactory("bot1", [startState]);

        var result = _factory.GetStateByTextsTypeOrDefault("bot1", 123L, "unknown");

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public void GetStateByTextsTypeOrDefault_WhenTextTypeValueNull_Throws()
    {
        CreateFactory("bot1", []);
        Assert.Throws<ArgumentNullException>(() =>
            _factory.GetStateByTextsTypeOrDefault("bot1", 123L, null!));
    }

    [Test]
    public async Task BindState_WhenStateExists_ReturnsSuccess()
    {
        var stateData = new StateFactoryData("Result");
        CreateFactory("bot1", [stateData]);
        var stateHistory = new StateHistory(typeof(Result));

        _cache.Setup(c => c.GetValueFromCollection<UserBindStateInCache>(
            It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((UserBindStateInCache?)default);
        _cache.Setup(c => c.AddValueToCollection(It.IsAny<string>(), It.IsAny<IKeyInCache>()))
            .Returns(Task.CompletedTask);

        var result = await _factory.BindState("bot1", 123L, stateHistory, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task BindState_WhenStateNotFound_ReturnsFailure()
    {
        CreateFactory("bot1", []);
        var stateHistory = new StateHistory(typeof(Result));

        var result = await _factory.BindState("bot1", 123L, stateHistory, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public async Task UnBindState_RemovesBindState()
    {
        CreateFactory("bot1", []);
        _cache.Setup(c => c.RemoveValueFromCollection(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await _factory.UnBindState("bot1", 123L, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        _cache.Verify(c => c.RemoveValueFromCollection(It.IsAny<string>(), "123"), Times.Once);
    }

    [Test]
    public async Task HasBindState_WhenBindStateExists_ReturnsTrue()
    {
        CreateFactory("bot1", []);
        var bindState = new UserBindStateInCache
        {
            ChatId = "123",
            StatesTypeName = "Result"
        };
        _cache.Setup(c => c.GetValueFromCollection<UserBindStateInCache>(
            It.IsAny<string>(), "123")).ReturnsAsync(bindState);

        var result = await _factory.HasBindState("bot1", 123L, CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasBindState_WhenBindStateDoesNotExist_ReturnsFalse()
    {
        CreateFactory("bot1", []);
        _cache.Setup(c => c.GetValueFromCollection<UserBindStateInCache>(
            It.IsAny<string>(), "123")).ReturnsAsync((UserBindStateInCache?)default);

        var result = await _factory.HasBindState("bot1", 123L, CancellationToken.None);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetBindStateOrNull_WhenBindStateExists_ReturnsState()
    {
        var stateData = new StateFactoryData("Result");
        CreateFactory("bot1", [stateData]);
        var bindState = new UserBindStateInCache
        {
            ChatId = "123",
            StatesTypeName = "Result"
        };
        _cache.Setup(c => c.GetValueFromCollection<UserBindStateInCache>(
            It.IsAny<string>(), "123")).ReturnsAsync(bindState);

        var result = await _factory.GetBindStateOrNull("bot1", 123L, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.StateType.Name, Is.EqualTo("Result"));
        }
    }

    [Test]
    public async Task GetBindStateOrNull_WhenBindStateDoesNotExist_ReturnsFailure()
    {
        CreateFactory("bot1", []);
        _cache.Setup(c => c.GetValueFromCollection<UserBindStateInCache>(
            It.IsAny<string>(), "123")).ReturnsAsync((UserBindStateInCache?)default);

        var result = await _factory.GetBindStateOrNull("bot1", 123L, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
    }

    [Test]
    public async Task GetStateByButtonsTypeOrDefault_WhenMatchExists_ReturnsState()
    {
        var stateData = new StateFactoryData("Result", buttonsTypes: ["button1"]);
        CreateFactory("bot1", [stateData]);
        _cache.Setup(c => c.GetValueFromCollection<UserStateInCache>(
            It.IsAny<string>(), "123")).ReturnsAsync((UserStateInCache?)default);

        var result = await _factory.GetStateByButtonsTypeOrDefault("bot1", 123L, "button1", CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.StateType.Name, Is.EqualTo("Result"));
        }
    }

    [Test]
    public async Task GetStateByCommandsTypeOrDefault_WhenMatchExists_ReturnsState()
    {
        var stateData = new StateFactoryData("Result", commandsTypes: ["/test"]);
        CreateFactory("bot1", [stateData]);
        _cache.Setup(c => c.GetValueFromCollection<UserStateInCache>(
            It.IsAny<string>(), "123")).ReturnsAsync((UserStateInCache?)default);

        var result = await _factory.GetStateByCommandsTypeOrDefault("bot1", 123L, "/test", CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.StateType.Name, Is.EqualTo("Result"));
        }
    }

    [Test]
    public async Task GetStateMain_ClearsStateHistory()
    {
        CreateFactory("bot1", []);
        var startState = new StateFactoryData("Result", commandsTypes: [CommandTypesConstant.StartCommand]);
        CreateFactory("bot1", [startState]);
        var existingState = new UserStateInCache
        {
            ChatId = "123",
            StatesTypeName = ["State1", "State2"]
        };
        _cache.Setup(c => c.GetValueFromCollection<UserStateInCache>(
            It.IsAny<string>(), "123")).ReturnsAsync(existingState);
        _cache.Setup(c => c.AddValueToCollection(It.IsAny<string>(), It.IsAny<IKeyInCache>()))
            .Returns(Task.CompletedTask);
        _cache.Setup(c => c.RemoveValueFromCollection(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await _factory.GetStateMain("bot1", 123L, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        _cache.Verify(c => c.AddValueToCollection(It.IsAny<string>(),
            It.Is<UserStateInCache>(v => v.StatesTypeName.Count == 0)), Times.Once);
    }

    [TearDown]
    public void TearDown() => _serviceProvider?.Dispose();
}
