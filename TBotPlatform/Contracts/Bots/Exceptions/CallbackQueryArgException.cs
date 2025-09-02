namespace TBotPlatform.Contracts.Bots.Exceptions;

public class CallbackQueryArgException() : ArgumentNullException(ErrorCode)
{
    private const string ErrorCode = "CallbackQuery";
}