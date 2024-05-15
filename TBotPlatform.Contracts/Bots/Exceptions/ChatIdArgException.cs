namespace TBotPlatform.Contracts.Bots.Exceptions;

public class ChatIdArgException() : ArgumentNullException(ErrorCode)
{
    private const string ErrorCode = "ChatId";
}