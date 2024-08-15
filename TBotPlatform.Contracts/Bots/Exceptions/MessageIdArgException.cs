namespace TBotPlatform.Contracts.Bots.Exceptions;

public class MessageIdArgException() : ArgumentNullException(ErrorCode)
{
    private const string ErrorCode = "MessageId";
}