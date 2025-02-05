#nullable enable
namespace TBotPlatform.Contracts.Statistics;

public class TelegramContextLogMessage
{
    public required Guid OperationGuid { get; set; }

    public required string OperationType { get; set; }

    public required long ChatId { get; set; }

    public required Dictionary<string, string> MessageBody { get; set; }
}