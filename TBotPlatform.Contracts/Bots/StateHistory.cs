namespace TBotPlatform.Contracts.Bots;

public class StateHistory
{
    /// <summary>
    /// Тип состояния
    /// </summary>
    public Type StateType { get; }

    /// <summary>
    /// Тип кнопок состояния
    /// </summary>
    public Type MenuStateType { get; }

    /// <summary>
    /// Состояние вызывается только с inline кнопок
    /// </summary>
    public bool IsInlineState { get; }

    public StateHistory(
        Type stateType,
        Type menuStateType = null,
        bool? isInlineState = null
        )
    {
        StateType = stateType;
        MenuStateType = menuStateType;

        if (isInlineState.HasValue)
        {
            IsInlineState = isInlineState.Value;
        }
    }
}