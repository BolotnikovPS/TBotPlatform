using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots.Buttons;

public class MainButton
{
    public MainButton(string button)
    {
        if (button.IsNull())
        {
            throw new ArgumentException("", button);
        }

        if (button.Length > ButtonsRuleConstant.ButtonsRuleNameLength)
        {
            ButtonName = button[..ButtonsRuleConstant.ButtonsRuleNameLength];
            return;
        }

        ButtonName = button;
    }

    /// <summary>
    /// Тип кнопки
    /// </summary>
    public string ButtonName { get; private set; }
}