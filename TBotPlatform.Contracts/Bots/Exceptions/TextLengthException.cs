namespace TBotPlatform.Contracts.Bots.Exceptions;

public class TextLengthException() : ArgumentException(ErrorCode)
{
    private const string ErrorCode = "TextLength";
}
