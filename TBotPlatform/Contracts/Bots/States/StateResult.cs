namespace TBotPlatform.Contracts.Bots.States;

public class StateResult
{
    /// <summary>
    /// Indicates whether the menu needs to be sent
    /// </summary>
    public bool IsNeedUpdateMarkup { get; set; }

    /// <summary>
    /// Name of the next state
    /// </summary>
    public string NextStateName { get; set; } = null!;

    /// <summary>
    /// Additional information about the state
    /// </summary>
    public string Data { get; set; } = null!;
}