namespace TBotPlatform.Contracts.Bots.Exceptions;

public class ReplyKeyboardMarkupArgException() : ArgumentNullException(ErrorCode)
{
    private const string ErrorCode = "ReplyKeyboardMarkup";
}