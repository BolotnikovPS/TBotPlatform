using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class StateActivatorBaseAttribute(bool isInlineState, Type menuType, bool isAdminState = false) : Attribute
{
    /// <summary>
    /// Перечень типов кнопок соответствующих состоянию
    /// </summary>
    public string[] ButtonsTypes { get; set; }

    /// <summary>
    /// Перечень типов текста соответствующих состоянию
    /// </summary>}
    public string[] TextsTypes { get; set; }

    /// <summary>
    /// Перечень типов команд соответствующих состоянию
    /// </summary>}
    public string[] CommandsTypes { get; set; }

    /// <summary>
    /// Тип меню, которое отображается пользователю, для данного состояния
    /// </summary>
    public Type MenuType { get; private set; } = menuType;

    /// <summary>
    /// Состояние вызывается только с inline кнопок
    /// </summary>
    public bool IsInlineState { get; private set; } = isInlineState;

    /// <summary>
    /// Состояние вызывается только при наличии блокировки у пользователя
    /// </summary>
    public bool IsLockUserState { get; set; }

    /// <summary>
    /// Показывает что состояние относится уровню регистрации пользователя
    /// </summary>
    public bool IsRegistrationState { get; set; }

    /// <summary>
    /// Показывает что состояние относится к пользователя <see cref="UserBase.IsAdmin"/>
    /// </summary>
    public bool IsAdminState { get; private set; } = isAdminState;

    /// <summary>
    /// Для какого бота доступно состояние
    /// </summary>
    public string OnlyForBot { get; set; }
}