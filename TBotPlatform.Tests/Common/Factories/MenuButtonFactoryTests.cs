using Microsoft.Extensions.DependencyInjection;
using Moq;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Tests.Common.Factories;

[TestFixture]
public class MenuButtonFactoryTests
{
    private MenuButtonFactory _factory = null!;
    private ServiceProvider _serviceProvider = null!;

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();
        services.AddScoped<IServiceScopeFactory, TestScopeFactory>();
        _serviceProvider = services.BuildServiceProvider();
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        _factory = new MenuButtonFactory(scopeFactory);
    }

    [Test]
    public void GetMainButtons_WhenStateHistoryMenuStateTypeOrNullIsNull_ThrowsArgumentNullException()
    {
        var user = new Mock<UserBase>().Object;
        var stateHistory = new StateHistory(typeof(object), null);

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _factory.GetMainButtons(user, stateHistory));
    }

    [Test]
    public void UpdateMainButtonsByState_WhenStateHistoryIsNull_ThrowsArgumentNullException()
    {
        var user = new Mock<UserBase>().Object;
        var stateContext = new Mock<TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.IStateContextMinimal>().Object;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _factory.UpdateMainButtonsByState(user, stateContext, (StateHistory)null!, CancellationToken.None));
    }

    [Test]
    public void UpdateMainButtonsByState_WhenStateHistoryMenuStateTypeOrNullIsNull_ThrowsArgumentNullException()
    {
        var user = new Mock<UserBase>().Object;
        var stateContext = new Mock<TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.IStateContextMinimal>().Object;
        var stateHistory = new StateHistory(typeof(object), null);

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _factory.UpdateMainButtonsByState(user, stateContext, stateHistory, CancellationToken.None));
    }

    [TearDown]
    public void TearDown() => _serviceProvider?.Dispose();

    private class TestScopeFactory : IServiceScopeFactory
    {
        public IServiceScope CreateScope() => new TestScope();
    }

    private class TestScope : IServiceScope
    {
        public IServiceProvider ServiceProvider { get; } = new ServiceCollection().BuildServiceProvider();

        public void Dispose()
        { }
    }
}
