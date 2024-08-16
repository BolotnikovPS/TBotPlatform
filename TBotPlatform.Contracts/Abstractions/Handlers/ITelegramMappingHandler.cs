using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Handlers;

public interface ITelegramMappingHandler
{
    ChatResult MappingMessage(Message message);
}