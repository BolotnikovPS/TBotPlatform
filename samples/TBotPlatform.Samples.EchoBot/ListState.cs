using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Entry point of the paginated list: shows the first page.
/// </summary>
/// <remarks>
/// A single class cannot combine <see cref="StateActivatorAttribute"/> and
/// <see cref="StateInlineActivatorAttribute"/>: the state builder reads only the first
/// found attribute, so page transitions are handled by a separate state
/// <see cref="ListPageState"/>.
/// </remarks>
[StateActivator(typeof(MainMenu), ButtonsTypes = ["Список"])]
internal sealed class ListState : BaseStateHandler<EchoUser>
{
    public override Task Handle(IStateContext context, EchoUser user, CancellationToken cancellationToken)
        => CityList.RenderPage(context, page: 1, cancellationToken);
}
