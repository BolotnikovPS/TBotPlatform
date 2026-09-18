using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;

namespace TBotPlatform.Samples.WebhookBot;

[StateActivator(typeof(MainMenu), ButtonsTypes = ["Ping"])]
internal sealed class PingState : BaseStateHandler<WebhookUser>
{
    public override Task Handle(IStateContext context, WebhookUser user, CancellationToken cancellationToken)
        => context.SendTextMessage("Pong", cancellationToken);
}
