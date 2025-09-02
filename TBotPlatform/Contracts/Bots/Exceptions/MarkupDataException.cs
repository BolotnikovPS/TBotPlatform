namespace TBotPlatform.Contracts.Bots.Exceptions;

public class MarkupDataException(string stateName) : ArgumentException(stateName, ErrorCode)
{
    private const string ErrorCode = nameof(MarkupNextState.Data);
}