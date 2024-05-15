using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Exceptions;

public class ReplyKeyboardMarkupArgException() : ArgumentNullException(ErrorCode)
{
    private const string ErrorCode = nameof(ReplyKeyboardMarkup);
}