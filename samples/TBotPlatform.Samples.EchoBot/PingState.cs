using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;

namespace TBotPlatform.Samples.EchoBot;

[StateActivator(typeof(MainMenu), ButtonsTypes = ["Ping"])]
internal sealed class PingState : BaseStateHandler<EchoUser>
{
    public override Task Handle(IStateContext context, EchoUser user, CancellationToken cancellationToken)
        => context.SendTextMessage("Pong", cancellationToken);
}
