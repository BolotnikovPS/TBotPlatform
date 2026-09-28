using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Точка входа в постраничный список: показывает первую страницу.
/// </summary>
/// <remarks>
/// На одном классе нельзя совмещать <see cref="StateActivatorAttribute"/> и
/// <see cref="StateInlineActivatorAttribute"/>: построитель состояний читает только первый
/// найденный атрибут, поэтому переходы по страницам обрабатывает отдельное состояние
/// <see cref="ListPageState"/>.
/// </remarks>
[StateActivator(typeof(MainMenu), ButtonsTypes = ["Список"])]
internal sealed class ListState : BaseStateHandler<EchoUser>
{
    public override Task Handle(IStateContext context, EchoUser user, CancellationToken cancellationToken)
        => CityList.RenderPage(context, page: 1, cancellationToken);
}
