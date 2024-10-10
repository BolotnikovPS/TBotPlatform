using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public Task<int> GetChatMemberCountAsync(long chatIdToCheck, CancellationToken cancellationToken)
    {
        if (chatIdToCheck.IsDefault())
        {
            throw new ChatIdArgException();
        }

        return telegramContext.GetChatMemberCountAsync(chatIdToCheck, cancellationToken);
    }

    public async Task<ChatMemberData> GetChatMemberAsync(long chatIdToCheck, long userIdToCheck, CancellationToken cancellationToken)
    {
        if (chatIdToCheck.IsDefault())
        {
            throw new ChatIdArgException();
        }

        var result = await telegramContext.GetChatMemberAsync(chatIdToCheck, userIdToCheck, cancellationToken);

        return telegramMapping.ChatMemberToData(result);
    }

    public async Task<List<ChatMemberData>> GetChatAdministratorsAsync(long chatIdToCheck, CancellationToken cancellationToken)
    {
        if (chatIdToCheck.IsDefault())
        {
            throw new ChatIdArgException();
        }

        var result = await telegramContext.GetChatAdministratorsAsync(chatIdToCheck, cancellationToken);

        return result.CheckAny() ? result.Select(telegramMapping.ChatMemberToData).ToList() : default;
    }

    public async Task<TelegramChat> GetChatAsync(long chatIdToCheck, CancellationToken cancellationToken)
    {
        if (chatIdToCheck.IsDefault())
        {
            throw new ChatIdArgException();
        }

        var result = await telegramContext.GetChatAsync(chatIdToCheck, cancellationToken);

        return telegramMapping.ChatToTelegramChat(result);
    }

    public Task LeaveChatAsync(long chatIdToLeave, CancellationToken cancellationToken)
    {
        if (chatIdToLeave.IsDefault())
        {
            throw new ChatIdArgException();
        }

        return telegramContext.LeaveChatAsync(chatId, cancellationToken);
    }
}