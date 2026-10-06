using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.StateFactory;

/// <summary>
/// State description.
/// </summary>
public class StateFactoryData
{
    /// <summary>
    /// List of button types matching the state.
    /// </summary>
    public List<string> ButtonsTypes { get; set; } = null!;

    /// <summary>
    /// List of text types matching the state.
    /// </summary>
    public List<string> TextsTypes { get; set; } = null!;

    /// <summary>
    /// List of command types matching the state.
    /// </summary>
    public List<string> CommandsTypes { get; set; } = null!;

    /// <summary>
    /// Name of the state type.
    /// </summary>
    public string StateTypeName { get; set; } = null!;

    /// <summary>
    /// State type, if known at registration time.
    /// </summary>
    public Type? StateType { get; set; }

    /// <summary>
    /// Name of the state button type.
    /// </summary>
    public string? MenuTypeName { get; set; }

    /// <summary>
    /// Menu type, if known at registration time.
    /// </summary>
    public Type? MenuType { get; set; }

    /// <summary>
    /// The state is invoked only from inline buttons.
    /// </summary>
    public bool IsInlineState { get; set; }

    /// <summary>
    /// The state is invoked only when the user is locked.
    /// </summary>
    public bool IsLockUserState { get; set; }

    /// <summary>
    /// Indicates the state belongs to the user registration level.
    /// </summary>
    public bool IsRegistrationState { get; set; }

    /// <summary>
    /// Indicates the state belongs to an admin user <see cref="UserBase.IsAdmin"/>.
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