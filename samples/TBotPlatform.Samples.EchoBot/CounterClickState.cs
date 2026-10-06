using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Handles presses on the counter button: recalculates the caption and replaces the keyboard
/// of the same message (without deleting it and sending it again).
/// </summary>
[StateInlineActivator]
internal sealed class CounterClickState : BaseStateHandler<EchoUser>
{
    public override Task Handle(IStateContext context, EchoUser user, CancellationToken cancellationToken)
    {
        var count = int.TryParse(context.MarkupNextState?.Data, out var value) ? value : 0;

        return context.UpdateInlineMarkup(Counter.CreateMarkup(count), cancellationToken);
    }
}
