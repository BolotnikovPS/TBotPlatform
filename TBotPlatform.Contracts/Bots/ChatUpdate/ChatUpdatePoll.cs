#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatUpdatePoll(
    string id,
    string question,
    ChatMessagePollOption[]? optionsOrNull,
    int totalVoterCount,
    bool isClosed,
    bool isAnonymous,
    string type,
    bool allowsMultipleAnswers,
    int? correctOptionId,
    string? explanationOrNull,
    ChatMessageEntity[]? explanationEntitiesOrNull,
    int? openPeriod,
    DateTime? closeDate
    )
{
    /// <summary>
    /// Unique poll identifier
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// Poll question, 1-300 characters
    /// </summary>
    public string Question { get; } = question;

    /// <summary>
    /// List of poll options
    /// </summary>
    public ChatMessagePollOption[]? OptionsOrNull { get; } = optionsOrNull;

    /// <summary>
    /// Total number of users that voted in the poll
    /// </summary>
    public int TotalVoterCount { get; } = totalVoterCount;

    /// <summary>
    /// <see langword="true"/>, if the poll is closed
    /// </summary>
    public bool IsClosed { get; } = isClosed;

    /// <summary>
    /// <see langword="true"/>, if the poll is anonymous
    /// </summary>
    public bool IsAnonymous { get; } = isAnonymous;

    /// <summary>
    /// Poll type, currently can be “regular” or “quiz”
    /// </summary>
    public string Type { get; } = type;

    /// <summary>
    /// <see langword="true"/>, if the poll allows multiple answers
    /// </summary>
    public bool AllowsMultipleAnswers { get; } = allowsMultipleAnswers;

    /// <summary>
    /// Optional. 0-based identifier of the correct answer option. Available only for polls in the quiz mode,
    /// which are closed, or was sent (not forwarded) by the bot or to the private chat with the bot.
    /// </summary>
    public int? CorrectOptionId { get; } = correctOptionId;

    /// <summary>
    /// Optional. Text that is shown when a user chooses an incorrect answer or taps on the lamp icon in a
    /// quiz-style poll, 0-200 characters
    /// </summary>
    public string? ExplanationOrNull { get; } = explanationOrNull;

    /// <summary>
    /// Optional. Special entities like usernames, URLs, bot commands, etc. that appear in the
    /// <see cref="ExplanationOrNull"/>
    /// </summary>
    public ChatMessageEntity[]? ExplanationEntitiesOrNull { get; } = explanationEntitiesOrNull;

    /// <summary>
    /// Optional. Amount of time in seconds the poll will be active after creation
    /// </summary>
    public int? OpenPeriod { get; } = openPeriod;

    /// <summary>
    /// Optional. Point in time when the poll will be automatically closed
    /// </summary>
    public DateTime? CloseDate { get; } = closeDate;
}