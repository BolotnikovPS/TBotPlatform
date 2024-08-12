using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatUpdatePollAnswer(string pollId, TelegramUser fromUser, int[] optionIds)
{
    /// <summary>
    /// Unique poll identifier
    /// </summary>
    public string PollId { get; } = pollId;

    /// <summary>
    /// The user, who changed the answer to the poll
    /// </summary>
    public TelegramUser FromUser { get; } = fromUser;

    /// <summary>
    /// 0-based identifiers of answer options, chosen by the user. May be empty if the user retracted their vote.
    /// </summary>
    public int[] OptionIds { get; } = optionIds;
}