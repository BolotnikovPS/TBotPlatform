using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public Task<int> GetChatMemberCount(long chatIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        return telegramContext.GetChatMemberCount(chatIdToCheck, cancellationToken);
    }

    public Task<ChatMember> GetChatMember(long chatIdToCheck, long userIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        return telegramContext.GetChatMember(chatIdToCheck, userIdToCheck, cancellationToken);
    }

    public async Task<List<ChatMember>> GetChatAdministrators(long chatIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        var result = await telegramContext.GetChatAdministrators(chatIdToCheck, cancellationToken);

        return result.CheckAny() ? result.ToList() : null;
    }

    public Task<ChatFullInfo> GetChat(long chatIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        return telegramContext.GetChat(chatIdToCheck, cancellationToken);
    }

    public Task LeaveChat(long chatIdToLeave, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToLeave);

        return telegramContext.LeaveChat(ChatId, cancellationToken);
    }
}