namespace TBotPlatform.Contracts.Bots.Exceptions;

public class TextLengthException(int currentLength, int maxLength)
    : ArgumentException($"Длина сообщения в {currentLength} больше максимальной длины в {maxLength}", ErrorCode)
{
    private const string ErrorCode = "TextLength";
}