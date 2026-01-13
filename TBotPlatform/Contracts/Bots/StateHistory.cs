#nullable enable
using TBotPlatform.Contracts.Bots.Users;

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

    /// <summary>
    /// Показывает что состояние относится к пользователя <see cref="UserBase.IsAdmin"/>
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