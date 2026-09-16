using TBotPlatform.Common;
using TBotPlatform.Common.Handlers.State;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots.ChatUpdate;

namespace TBotPlatform.Samples.EchoBot;

internal sealed class EchoReceivingHandler(IStateFactory stateFactory, IStateContextFactory stateContextFactory)
    : StartReceivingHandlerBase<EchoUser>(stateFactory, stateContextFactory)
{
    protected override Task<EchoUser> GetOrCreateUser(string botName, TelegramMessageUserData telegramData, CancellationToken cancellationToken)
    {
        var user = telegramData.UserOrNull;
        var chat = telegramData.ChatOrNull;

        return Task.FromResult(new EchoUser
        {
            ChatId = chat?.Id ?? 0,
            TgUserId = user?.Id ?? 0,
            UserName = user?.Username,
            FirstName = user?.FirstName,
            LastName = user?.LastName,
        });
    }
}
