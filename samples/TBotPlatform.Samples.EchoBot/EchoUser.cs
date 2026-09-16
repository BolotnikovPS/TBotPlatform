using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Samples.EchoBot;

internal sealed class EchoUser : UserBase
{
    public override bool IsAdmin() => false;
}
