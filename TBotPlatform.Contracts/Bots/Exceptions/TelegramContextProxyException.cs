namespace TBotPlatform.Contracts.Bots.Exceptions;

public class TelegramContextProxyException() : ArgumentNullException(ErrorCode)
{
    private const string ErrorCode = " TelegramContextProxy";
}