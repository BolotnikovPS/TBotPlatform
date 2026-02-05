using Moq;
using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Tests.Common.Handlers.State;

[TestFixture]
public class BaseStateHandlerTests
{
    [Test]
    public async Task HandleComplete_ByDefault_ReturnsCompletedTask()
    {
        var handler = new TestStateHandler();
        var context = new Mock<IStateContext>().Object;
        var user = new TestUser();

        var task = handler.HandleComplete(context, user, CancellationToken.None);
        await task;

        Assert.That(task.IsCompletedSuccessfully, Is.True);
    }

    [Test]
    public async Task HandleError_ByDefault_ReturnsCompletedTask()
    {
        var handler = new TestStateHandler();
        var context = new Mock<IStateContext>().Object;
        var user = new TestUser();

        var task = handler.HandleError(context, user, new Exception("test"), CancellationToken.None);
        await task;

        Assert.That(task.IsCompletedSuccessfully, Is.True);
    }

    [Test]
    public async Task Handle_WhenImplemented_IsInvoked()
    {
        var handler = new TestStateHandler();
        var context = new Mock<IStateContext>().Object;
        var user = new TestUser();

        await handler.Handle(context, user, CancellationToken.None);

        Assert.That(handler.HandleCalled, Is.True);
    }

    private class TestUser : UserBase
    {
        public override bool IsAdmin() => false;
    }

    private class TestStateHandler : BaseStateHandler<TestUser>
    {
        public bool HandleCalled { get; private set; }

        public override Task Handle(IStateContext context, TestUser user, CancellationToken cancellationToken)
        {
            HandleCalled = true;
            return Task.CompletedTask;
        }
    }
}
