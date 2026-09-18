using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;

namespace TBotPlatform.Samples.WebhookBot;

[StateInlineActivator]
internal sealed class InlineYesState : BaseStateHandler<WebhookUser>
{
    public override Task Handle(IStateContext context, WebhookUser user, CancellationToken cancellationToken)
        => context.SendTextMessage("Вы выбрали: Да 👍", cancellationToken);
}
