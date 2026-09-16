using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots.Constant;

namespace TBotPlatform.Samples.EchoBot;

[StateActivator(typeof(MainMenu), CommandsTypes = [CommandTypesConstant.StartCommand])]
internal sealed class StartState : BaseStateHandler<EchoUser>
{
    public override Task Handle(IStateContext context, EchoUser user, CancellationToken cancellationToken)
    {
        _ = context.MakeDelayRequest(
            TimeSpan.FromSeconds(5),
            stateContext => stateContext.SendTextMessage("⏰ Это сообщение пришло с задержкой 5 секунд.", CancellationToken.None)
            );

        return context.SendTextMessage("Привет! Нажмите Ping или Инфо, либо напишите /start.", cancellationToken);
    }
}
