#nullable enable
namespace TBotPlatform.Contracts.Bots.Buttons;

public class MainButtonMassiveList : List<MainButtonMassive>
{
    public ButtonsRule? ButtonsRule { get; set; } = new()
    {
        IsPersistent = true,
        ResizeKeyboard = true,
    };
}