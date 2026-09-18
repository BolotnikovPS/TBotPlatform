using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

namespace TBotPlatform.Samples.WebhookBot;

[StateActivator(typeof(MainMenu), ButtonsTypes = ["Инфо"])]
internal sealed class InfoState : BaseStateHandler<WebhookUser>
{
    public override Task Handle(IStateContext context, WebhookUser user, CancellationToken cancellationToken)
        => context.SendOrUpdateTextMessage(
            "Выберите ответ:",
            new InlineMarkupList
            {
                new InlineMarkupState("Да", nameof(InlineYesState)),
                new InlineMarkupState("Нет", nameof(InlineNoState)),
            },
            cancellationToken);
}
