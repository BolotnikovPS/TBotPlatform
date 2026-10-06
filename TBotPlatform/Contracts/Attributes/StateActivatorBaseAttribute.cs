using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class StateActivatorBaseAttribute(bool isInlineState, Type? menuType, bool isAdminState = false) : Attribute
{
    /// <summary>
    /// List of button types corresponding to the state
    /// </summary>
    public string[] ButtonsTypes { get; set; } = null!;

    /// <summary>
    /// List of text types corresponding to the state
    /// </summary>
    public string[] TextsTypes { get; set; } = null!;

    /// <summary>
    /// List of command types corresponding to the state
    /// </summary>
    public string[] CommandsTypes { get; set; } = null!;

    /// <summary>
    /// Menu type displayed to the user for this state
    /// </summary>
    public Type MenuType { get; private set; } = menuType!;

    /// <summary>
    /// The state is invoked only from inline buttons
    /// </summary>
    public bool IsInlineState { get; private set; } = isInlineState;

    /// <summary>
    /// The state is invoked only when the user is locked
    /// </summary>
    public bool IsLockUserState { get; set; }

    /// <summary>
    /// Indicates that the state belongs to the user registration level
    /// </summary>
    public bool IsRegistrationState { get; set; }

    /// <summary>
    /// Indicates that the state belongs to the user <see cref="UserBase.IsAdmin"/>
    /// </summary>
    public bool IsAdminState { get; private set; } = isAdminState;

    /// <summary>
    /// For which bot the state is available
    /// </summary>
    public string OnlyForBot { get; set; } = null!;
}