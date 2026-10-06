#nullable enable
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots;

public class StateHistory
{
    /// <summary>
    /// State type
    /// </summary>
    public Type StateType { get; }

    /// <summary>
    /// State buttons type
    /// </summary>
    public Type? MenuStateTypeOrNull { get; }

    /// <summary>
    /// Indicates that the state is invoked only from inline buttons
    /// </summary>
    public bool IsInlineState { get; }

    /// <summary>
    /// Indicates that the state belongs to users with <see cref="UserBase.IsAdmin"/>
    /// </summary>
    public bool IsAdminState { get; set; }

    public StateHistory(Type stateType, Type? menuStateTypeOrNull = null, bool? isInlineState = null, bool? isAdminState = null)
    {
        StateType = stateType;
        MenuStateTypeOrNull = menuStateTypeOrNull;

        if (isInlineState.HasValue)
        {
            IsInlineState = isInlineState.Value;
        }

        if (isAdminState.HasValue)
        {
            IsAdminState = isAdminState.Value;
        }
    }
}