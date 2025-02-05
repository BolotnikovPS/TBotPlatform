using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    public Task<int> GetChatMemberCountAsync(long chatIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        return telegramContext.GetChatMemberCount(chatIdToCheck, cancellationToken);
    }

    public Task<ChatMember> GetChatMemberAsync(long chatIdToCheck, long userIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        return telegramContext.GetChatMember(chatIdToCheck, userIdToCheck, cancellationToken);
    }

    public async Task<List<ChatMember>> GetChatAdministratorsAsync(long chatIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        var result = await telegramContext.GetChatAdministrators(chatIdToCheck, cancellationToken);

        return result.CheckAny() ? result.ToList() : default;
    }

    public Task<ChatFullInfo> GetChatAsync(long chatIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        return telegramContext.GetChat(chatIdToCheck, cancellationToken);
    }

    public Task LeaveChatAsync(long chatIdToLeave, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToLeave);

        return telegramContext.LeaveChat(ChatId, cancellationToken);
    }
}