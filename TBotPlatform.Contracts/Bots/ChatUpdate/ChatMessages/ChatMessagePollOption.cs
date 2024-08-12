namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

public class ChatMessagePollOption(string text, int voterCount)
{
    /// <summary>
    /// Option text, 1-100 characters
    /// </summary>
    public string Text { get; } = text;

    /// <summary>
    /// Number of users that voted for this option
    /// </summary>
    public int VoterCount { get; } = voterCount;
}