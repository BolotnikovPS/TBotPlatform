using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Bots.ChatMessages;

public class TelegramUser(User tgUser, Chat chat)
{
    public User User { get; } = tgUser;
    public Chat Chat { get; } = chat;
}