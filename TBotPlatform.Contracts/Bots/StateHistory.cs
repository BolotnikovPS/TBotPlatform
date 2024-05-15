using TBotPlatform.Contracts.Abstractions;

namespace TBotPlatform.Contracts.Bots;

public class StateHistory<T>
where T : UserBase
{
    /// <summary>
    /// Тип состояния
    /// </summary>
    public Type StateType { get; }

    /// <summary>
    /// Кнопки состояния
    /// </summary>
    public IMenuButton<T> MenuState { get; }

    /// <summary>
    /// Состояние вызывается только с инлайн кнопок
    /// </summary>
    public bool IsInlineState { get; }

    public StateHistory(
        Type stateType,
        IMenuButton<T> menuState = null,
        bool? isInlineState = null
        )
    {
        StateType = stateType;
        MenuState = menuState;

        if (isInlineState.HasValue)
        {
            IsInlineState = isInlineState.Value;
        }
    }
}