using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Обрабатывает нажатия на кнопки перехода по страницам списка.
/// </summary>
/// <remarks>
/// Сообщение не переотправляется: <c>SendOrUpdateTextMessage</c> редактирует то же сообщение,
/// на кнопке которого был callback (позиция в истории чата сохраняется).
/// </remarks>
[StateInlineActivator]
internal sealed class ListPageState : BaseStateHandler<EchoUser>
{
    public override Task Handle(IStateContext context, EchoUser user, CancellationToken cancellationToken)
    {
        var page = context.MarkupNextState!.TryParsePagination(out var position) ? position : 1;

        return CityList.RenderPage(context, page, cancellationToken);
    }
}
