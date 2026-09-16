using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.StateFactory;

/// <summary>
/// Описание состояния
/// </summary>
public class StateFactoryData
{
    /// <summary>
    /// Перечень типов кнопок соответствующих состоянию
    /// </summary>
    public List<string> ButtonsTypes { get; set; } = null!;

    /// <summary>
    /// Перечень типов текста соответствующих состоянию
    /// </summary>
    public List<string> TextsTypes { get; set; } = null!;

    /// <summary>
    /// Перечень типов команд соответствующих состоянию
    /// </summary>
    public List<string> CommandsTypes { get; set; } = null!;

    /// <summary>
    /// Наименование типа состояния
    /// </summary>
    public string StateTypeName { get; set; } = null!;

    /// <summary>
    /// Тип состояния, если известен на этапе регистрации
    /// </summary>
    public Type? StateType { get; set; }

    /// <summary>
    /// Наименование типа кнопок состояния
    /// </summary>
    public string? MenuTypeName { get; set; }

    /// <summary>
    /// Тип меню, если известен на этапе регистрации
    /// </summary>
    public Type? MenuType { get; set; }

    /// <summary>
    /// Состояние вызывается только с inline кнопок
    /// </summary>
    public bool IsInlineState { get; set; }

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
    public bool IsAdminState { get; set; }

    public StateFactoryData(
        Type stateType,
        Type? menuType = null,
        bool? isInlineState = null,
        bool? isLockState = null,
        bool? isRegistrationState = null,
        bool? isAdminState = null,
        IEnumerable<string>? buttonsTypes = null,
        IEnumerable<string>? textsTypes = null,
        IEnumerable<string>? commandsTypes = null
        ) : this(
            stateType.Name,
            menuType?.Name,
            isInlineState,
            isLockState,
            isRegistrationState,
            isAdminState,
            buttonsTypes,
            textsTypes,
            commandsTypes)
    {
        StateType = stateType;
        MenuType = menuType;
    }

    public StateFactoryData(
        string stateTypeName,
        string? menuTypeName = null,
        bool? isInlineState = null,
        bool? isLockState = null,
        bool? isRegistrationState = null,
        bool? isAdminState = null,
        IEnumerable<string>? buttonsTypes = null,
        IEnumerable<string>? textsTypes = null,
        IEnumerable<string>? commandsTypes = null
        )
    {
        ButtonsTypes = buttonsTypes?.ToList() ?? [];
        TextsTypes = textsTypes?.ToList() ?? [];
        CommandsTypes = commandsTypes?.ToList() ?? [];
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

        if (isRegistrationState.HasValue)
        {
            IsRegistrationState = isRegistrationState.Value;
        }

        if (isAdminState.HasValue)
        {
            IsAdminState = isAdminState.Value;
        }
    }
}