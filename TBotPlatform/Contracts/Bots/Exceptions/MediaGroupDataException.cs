namespace TBotPlatform.Contracts.Bots.Exceptions;

public class MediaGroupDataException() : ArgumentNullException(ErrorCode)
{
    private const string ErrorCode = "MediaData";
}
