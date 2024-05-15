namespace TBotPlatform.Contracts.Bots.StateFactory;

/// <summary>
/// Описание состояния
/// </summary>
public class StateFactoryData
{
    /// <summary>
    /// Перечень типов кнопок соответствующих состоянию
    /// </summary>
    public List<string> ButtonsTypes { get; set; }

    /// <summary>
    /// Перечень типов текста соответствующих состоянию
    /// </summary>
    public List<string> TextsTypes { get; set; }

    /// <summary>
    /// Перечень типов комманд соответствующих состоянию
    /// </summary>
    public List<string> CommandsTypes { get; set; }

    /// <summary>
    /// Наименование типа состояния
    /// </summary>
    public string StateTypeName { get; set; }

    /// <summary>
    /// Наименование типа кнопок состояния
    /// </summary>
    public string MenuTypeName { get; set; }

    /// <summary>
    /// Состояние вызывается только с инлайн кнопок
    /// </summary>
    public bool IsInlineState { get; set; }

    /// <summary>
    /// Состояние вызывается только при наличии блокировки у пользователя
    /// </summary>
    public bool IsLockUserState { get; set; }

    public StateFactoryData(
        string stateTypeName,
        string menuTypeName = null,
        bool? isInlineState = null,
        bool? isLockState = null,
        IEnumerable<string> buttonsTypes = null,
        IEnumerable<string> textsTypes = null,
        IEnumerable<string> commandsTypes = null
        )
    {
        ButtonsTypes = buttonsTypes?.ToList();
        TextsTypes = textsTypes?.ToList();
        CommandsTypes = commandsTypes?.ToList();
        StateTypeName = stateTypeName;
        MenuTypeName = menuTypeName;

        if (isInlineState.HasValue)
        {
            IsInlineState = isInlineState.Value;
        }

        if (isLockState.HasValue)
        {
            IsLockUserState = isLockState.Value;
        }
    }
}