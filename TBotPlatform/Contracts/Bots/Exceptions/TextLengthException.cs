namespace TBotPlatform.Contracts.Bots.Exceptions;

public class TextLengthException(int currentLength, int maxLength)
    : ArgumentException($"Message length {currentLength} exceeds the maximum length of {maxLength}", ErrorCode)
{
    private const string ErrorCode = "TextLength";
}