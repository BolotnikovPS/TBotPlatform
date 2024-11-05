#nullable enable
namespace TBotPlatform.Contracts.Statistics;

public class TelegramContextLogMessage
{
    public required Guid OperationGuid { get; set; }

    public required string OperationType { get; set; }

    public required long ChatId { get; set; }

    public required TelegramContextLogMessageData MessageBody { get; set; }
}