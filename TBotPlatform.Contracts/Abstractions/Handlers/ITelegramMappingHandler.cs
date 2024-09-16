using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Handlers;

public interface ITelegramMappingHandler
{
    ChatResult MessageToResult(Message message);
    ChatMemberData ChatMemberToData(ChatMember chatMember);
    TelegramChat ChatToTelegramChat(Chat chat);
}