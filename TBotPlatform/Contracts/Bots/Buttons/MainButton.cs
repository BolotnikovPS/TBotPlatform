using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots.Buttons;

public class MainButton(string button)
{
    /// <summary>
    /// Тип кнопки
    /// </summary>
    public string ButtonName { get; set; } = button.IsNull()
        ? throw new ArgumentException("", button)
        : button;
}