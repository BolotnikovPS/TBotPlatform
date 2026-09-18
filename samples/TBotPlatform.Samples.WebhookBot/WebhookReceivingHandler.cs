using TBotPlatform.Common;
using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots.ChatUpdate;

namespace TBotPlatform.Samples.WebhookBot;

internal sealed class WebhookReceivingHandler(IStateFactory stateFactory, IStateContextFactory stateContextFactory)
    : StartReceivingHandlerBase<WebhookUser>(stateFactory, stateContextFactory)
{
    protected override Task<WebhookUser> GetOrCreateUser(string botName, TelegramMessageUserData telegramData, CancellationToken cancellationToken)
    {
        var user = telegramData.UserOrNull;
        var chat = telegramData.ChatOrNull;

        return Task.FromResult(new WebhookUser
        {
            ChatId = chat?.Id ?? 0,
            TgUserId = user?.Id ?? 0,
            UserName = user?.Username,
            FirstName = user?.FirstName,
            LastName = user?.LastName,
        });
    }
}
