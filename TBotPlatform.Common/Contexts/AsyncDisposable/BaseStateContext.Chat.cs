using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    public Task<int> GetChatMemberCountAsync(long chatIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        return telegramContext.GetChatMemberCountAsync(chatIdToCheck, cancellationToken);
    }

    public async Task<ChatMemberData> GetChatMemberAsync(long chatIdToCheck, long userIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        var result = await telegramContext.GetChatMemberAsync(chatIdToCheck, userIdToCheck, cancellationToken);

        return telegramMapping.ChatMemberToData(result);
    }

    public async Task<List<ChatMemberData>> GetChatAdministratorsAsync(long chatIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        var result = await telegramContext.GetChatAdministratorsAsync(chatIdToCheck, cancellationToken);

        return result.CheckAny() ? result.Select(telegramMapping.ChatMemberToData).ToList() : default;
    }

    public async Task<TelegramChatFullInfo> GetChatAsync(long chatIdToCheck, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToCheck);

        var result = await telegramContext.GetChatAsync(chatIdToCheck, cancellationToken);

        return telegramMapping.ChatToTelegramChat(result);
    }

    public Task LeaveChatAsync(long chatIdToLeave, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow(chatIdToLeave);

        return telegramContext.LeaveChatAsync(ChatId, cancellationToken);
    }
}