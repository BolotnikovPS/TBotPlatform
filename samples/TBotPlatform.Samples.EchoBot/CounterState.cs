using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Entry point of the counter: sends a message with an inline button.
/// </summary>
[StateActivator(typeof(MainMenu), ButtonsTypes = ["Счётчик"])]
internal sealed class CounterState : BaseStateHandler<EchoUser>
{
    public override Task Handle(IStateContext context, EchoUser user, CancellationToken cancellationToken)
        => context.SendOrUpdateTextMessage(
            "Нажмите кнопку: текст сообщения не изменится, обновится только клавиатура (UpdateInlineMarkup).",
            Counter.CreateMarkup(count: 0),
            cancellationToken);
}
