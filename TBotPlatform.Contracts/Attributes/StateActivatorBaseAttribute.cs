namespace TBotPlatform.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class StateActivatorBaseAttribute : Attribute
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
    /// Перечень типов комманд соответствующих состоянию
    /// </summary>}
    public string[] CommandsTypes { get; set; }

    /// <summary>
    /// Тип меню, которое отображается пользователю, для данного состояния
    /// </summary>
    public Type MenuType { get; set; }

    /// <summary>
    /// Состояние вызывается только с инлайн кнопок
    /// </summary>
    public bool IsInlineState { get; set; }

    /// <summary>
    /// Состояние вызывается только при наличии блокировки у пользователя
    /// </summary>
    public bool IsLockUserState { get; set; }

    /// <summary>
    /// Для какого бота доступно состояние
    /// </summary>
    public string OnlyForBot { get; set; }
}