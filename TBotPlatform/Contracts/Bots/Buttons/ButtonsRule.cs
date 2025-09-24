#nullable enable
namespace TBotPlatform.Contracts.Bots.Buttons;

public class ButtonsRule
{
    /// <summary>
    /// Optional. Requests clients to always show the keyboard when the regular keyboard is hidden. Defaults to
    /// <see langword="false"/>, in which case the custom keyboard can be hidden and opened with a keyboard icon.
    /// </summary>
    public bool? IsPersistent { get; set; }

    /// <summary>
    /// Optional. Requests clients to resize the keyboard vertically for optimal fit (e.g., make the keyboard smaller if there are just two rows of buttons). Defaults to false, in which case the custom keyboard is always of the same height as the app's standard keyboard.
    /// </summary>
    public bool? ResizeKeyboard { get; set; }

    /// <summary>
    /// Optional. Requests clients to hide the keyboard as soon as it's been used. The keyboard will still be available, but clients will automatically display the usual letter-keyboard in the chat – the user can press a special button in the input field to see the custom keyboard again. Defaults to false.
    /// </summary>
    public bool? OneTimeKeyboard { get; set; }

    /// <summary>
    /// Optional. The placeholder to be shown in the input field when the keyboard is active; 1-64 characters
    /// </summary>
    public string? InputFieldPlaceholder { get; set; }

    public bool? Selective { get; set; }
}