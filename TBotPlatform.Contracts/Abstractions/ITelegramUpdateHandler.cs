using TBotPlatform.Contracts.Bots.ChatMessages;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions;

public interface ITelegramUpdateHandler
{
    TelegramUser GetTelegramUser(Update update);

    Task<ChatMessage> GetChatMessageAsync(TelegramUser telegramUser, Update update, CancellationToken cancellationToken);

    Task<ChatMessage> GetChatMessageAsync(long chatId, Update update, CancellationToken cancellationToken);
}