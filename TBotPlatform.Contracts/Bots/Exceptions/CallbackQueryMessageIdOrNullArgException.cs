namespace TBotPlatform.Contracts.Bots.Exceptions;

public class CallbackQueryMessageIdOrNullArgException() : ArgumentNullException(ErrorCode)
{
    private const string ErrorCode = "CallbackQueryMessageIdOrNull";
}