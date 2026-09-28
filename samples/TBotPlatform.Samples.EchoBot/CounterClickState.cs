using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Обрабатывает нажатия на кнопку счётчика: пересчитывает подпись и заменяет клавиатуру
/// у того же сообщения (без удаления и повторной отправки).
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
