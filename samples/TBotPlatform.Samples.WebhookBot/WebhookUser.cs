using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Samples.WebhookBot;

internal sealed class WebhookUser : UserBase
{
    public override bool IsAdmin() => false;
}
