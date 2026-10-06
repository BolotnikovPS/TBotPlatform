using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Contracts.Cache;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform.Tests.Common.Factories;

/// <summary>
/// Tests the caching part of <see cref="StateFactory"/>: the user state stack,
/// the storage limit, state binding, and cancellation handling.
/// </summary>
[TestFixture]
public class StateFactoryCacheTests
{
    private const string BotName = "cache-bot";
    private const long ChatIdValue = 555;
    private const string MenuButton = "Меню";
    private const string InfoButton = "Инфо";
    private const string SilentButton = "Тихий";
    private const string InlineButton = "Инлайн";

    private ServiceProvider _provider = null!;
    private StateFactory _factory = null!;

    private IFusionCache FusionCache => _provider.GetRequiredService<IFusionCache>();

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();
        services.AddFusionCache();
        services.AddKeyedSingleton(BotName, CreateCollection());
        _provider = services.BuildServiceProvider();

        _factory = new StateFactory(FusionCache, _provider, typeof(StateFactoryCacheTests).Assembly);
    }

    [TearDown]
    public async Task TearDown() => await _provider.DisposeAsync();

    [Test]
    public async Task GetStateByButtonsTypeOrDefault_WhenNotInlineState_StoresStateInCache()
    {
        var result = await _factory.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, InfoButton, CancellationToken.None);
        var states = await GetCachedStates();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.StateType, Is.EqualTo(typeof(CacheInfoState)));
            Assert.That(states, Is.EqualTo(new[] { nameof(CacheInfoState) }));
        }
    }

    [Test]
    public async Task GetStateByButtonsTypeOrDefault_WhenSameStatePressedTwice_KeepsSingleEntry()
    {
        await _factory.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, InfoButton, CancellationToken.None);
        await _factory.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, InfoButton, CancellationToken.None);

        var states = await GetCachedStates();

        Assert.That(states, Is.EqualTo(new[] { nameof(CacheInfoState) }));
    }

    [Test]
    public async Task GetStateByButtonsTypeOrDefault_WhenInlineState_DoesNotStoreState()
    {
        var result = await _factory.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, InlineButton, CancellationToken.None);
        var states = await GetCachedStates();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(states, Is.Empty);
        }
    }

    [Test]
    public async Task GetStateByCommandsTypeOrDefault_WhenCommandMatches_StoresStateInCache()
    {
        var result = await _factory.GetStateByCommandsTypeOrDefault(
            BotName,
            ChatIdValue,
            CommandTypesConstant.StartCommand,
            CancellationToken.None);

        var states = await GetCachedStates();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.StateType, Is.EqualTo(typeof(CacheStartState)));
            Assert.That(states, Is.EqualTo(new[] { nameof(CacheStartState) }));
        }
    }

    [Test]
    public async Task GetStateByButtonsTypeOrDefault_WhenStatesExceedLimit_KeepsOnlyLastTen()
    {
        for (var index = 0; index < 12; index++)
        {
            var button = index % 2 == 0 ? MenuButton : InfoButton;

            await _factory.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, button, CancellationToken.None);
        }

        var states = await GetCachedStates();

        string[] expected =
        [
            nameof(CacheMenuState),
            nameof(CacheInfoState),
            nameof(CacheMenuState),
            nameof(CacheInfoState),
            nameof(CacheMenuState),
            nameof(CacheInfoState),
            nameof(CacheMenuState),
            nameof(CacheInfoState),
            nameof(CacheMenuState),
            nameof(CacheInfoState),
        ];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(states, Has.Count.EqualTo(10));
            Assert.That(states, Is.EqualTo(expected));
        }
    }

    [Test]
    public async Task GetStateMain_WhenStatesStored_ClearsCacheAndReturnsStartState()
    {
        await _factory.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, InfoButton, CancellationToken.None);

        var result = await _factory.GetStateMain(BotName, ChatIdValue, CancellationToken.None);
        var states = await GetCachedStates();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.StateType, Is.EqualTo(typeof(CacheStartState)));
            Assert.That(states, Is.Empty);
        }
    }

    [Test]
    public async Task GetStatePreviousOrMain_WhenStatesStored_ReturnsPreviousStateWithMenu()
    {
        await _factory.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, MenuButton, CancellationToken.None);
        await _factory.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, InfoButton, CancellationToken.None);

        var result = await _factory.GetStatePreviousOrMain(BotName, ChatIdValue, CancellationToken.None);
        var states = await GetCachedStates();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.StateType, Is.EqualTo(typeof(CacheMenuState)));
            Assert.That(states, Is.EqualTo(new[] { nameof(CacheMenuState) }));
        }
    }

    [Test]
    public async Task GetStatePreviousOrMain_WhenCacheEmpty_ReturnsStartState()
    {
        var result = await _factory.GetStatePreviousOrMain(BotName, ChatIdValue, CancellationToken.None);
        var states = await GetCachedStates();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.StateType, Is.EqualTo(typeof(CacheStartState)));
            Assert.That(states, Is.Empty);
        }
    }

    [Test]
    public async Task GetLastStateWithMenu_WhenLastStateHasNoMenu_SkipsItAndReturnsMenuState()
    {
        await _factory.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, MenuButton, CancellationToken.None);
        await _factory.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, SilentButton, CancellationToken.None);

        var result = await _factory.GetLastStateWithMenu(BotName, ChatIdValue, CancellationToken.None);
        var states = await GetCachedStates();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.StateType, Is.EqualTo(typeof(CacheMenuState)));
            Assert.That(states, Is.EqualTo(new[] { nameof(CacheMenuState) }));
        }
    }

    [Test]
    public async Task BindState_ThenGetBindStateOrNull_ReturnsBoundState()
    {
        var bindResult = await _factory.BindState(BotName, ChatIdValue, new StateHistory(typeof(CacheInfoState)), CancellationToken.None);
        var hasBindState = await _factory.HasBindState(BotName, ChatIdValue, CancellationToken.None);
        var bindState = await _factory.GetBindStateOrNull(BotName, ChatIdValue, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(bindResult.IsSuccess, Is.True);
            Assert.That(hasBindState.IsSuccess, Is.True);
            Assert.That(bindState.IsSuccess, Is.True);
            Assert.That(bindState.Value.StateType, Is.EqualTo(typeof(CacheInfoState)));
        }
    }

    [Test]
    public async Task BindState_WhenAnotherStateBound_ReplacesPreviousBinding()
    {
        await _factory.BindState(BotName, ChatIdValue, new StateHistory(typeof(CacheMenuState)), CancellationToken.None);
        await _factory.BindState(BotName, ChatIdValue, new StateHistory(typeof(CacheInfoState)), CancellationToken.None);

        var bindState = await _factory.GetBindStateOrNull(BotName, ChatIdValue, CancellationToken.None);

        Assert.That(bindState.Value.StateType, Is.EqualTo(typeof(CacheInfoState)));
    }

    [Test]
    public async Task UnBindState_AfterBind_RemovesBinding()
    {
        await _factory.BindState(BotName, ChatIdValue, new StateHistory(typeof(CacheInfoState)), CancellationToken.None);

        var unbindResult = await _factory.UnBindState(BotName, ChatIdValue, CancellationToken.None);
        var hasBindState = await _factory.HasBindState(BotName, ChatIdValue, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(unbindResult.IsSuccess, Is.True);
            Assert.That(hasBindState.IsSuccess, Is.False);
        }
    }

    [Test]
    public async Task GetStateByButtonsTypeOrDefault_WhenBindingExists_RemovesBinding()
    {
        await _factory.BindState(BotName, ChatIdValue, new StateHistory(typeof(CacheInfoState)), CancellationToken.None);

        await _factory.GetStateByButtonsTypeOrDefault(BotName, ChatIdValue, MenuButton, CancellationToken.None);

        var hasBindState = await _factory.HasBindState(BotName, ChatIdValue, CancellationToken.None);
        var states = await GetCachedStates();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hasBindState.IsSuccess, Is.False);
            Assert.That(states, Is.EqualTo(new[] { nameof(CacheMenuState) }));
        }
    }

    [Test]
    public async Task GetStateMain_WhenTokenCancelled_ThrowsOperationCanceledException()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _factory.GetStateMain(BotName, ChatIdValue, cancellationTokenSource.Token));
    }

    private async Task<List<string>> GetCachedStates()
    {
        var cached = await FusionCache.GetValueFromCollection<UserStateInCache>($"{BotName}_UserStates", ChatIdValue.ToString());

        return cached.IsSuccess ? cached.Value.StatesTypeName : [];
    }

    private static StateFactoryDataCollection CreateCollection()
        => new([
            new StateFactoryData(typeof(CacheStartState), typeof(CacheMenuState), commandsTypes: [CommandTypesConstant.StartCommand]),
            new StateFactoryData(typeof(CacheMenuState), typeof(CacheMenuState), buttonsTypes: [MenuButton]),
            new StateFactoryData(typeof(CacheInfoState), typeof(CacheMenuState), buttonsTypes: [InfoButton]),
            new StateFactoryData(typeof(CacheSilentState), buttonsTypes: [SilentButton]),
            new StateFactoryData(typeof(CacheInlineState), typeof(CacheMenuState), isInlineState: true, buttonsTypes: [InlineButton]),
        ]);

    private sealed class CacheStartState { }

    private sealed class CacheMenuState { }

    private sealed class CacheInfoState { }

    private sealed class CacheSilentState { }

    private sealed class CacheInlineState { }
}
