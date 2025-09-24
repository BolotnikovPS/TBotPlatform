#nullable enable
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
    public Type? MenuStateTypeOrNull { get; }

    /// <summary>
    /// Состояние вызывается только с inline кнопок
    /// </summary>
    public bool IsInlineState { get; }

    public StateHistory(Type stateType, Type? menuStateTypeOrNull = null, bool? isInlineState = null)
    {
        StateType = stateType;
        MenuStateTypeOrNull = menuStateTypeOrNull;

        if (isInlineState.HasValue)
        {
            IsInlineState = isInlineState.Value;
        }
    }
}