using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Handles presses on the buttons that navigate through the pages of the list.
/// </summary>
/// <remarks>
/// The message is not re-sent: <c>SendOrUpdateTextMessage</c> edits the same message
/// whose button triggered the callback (its position in the chat history is preserved).
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
