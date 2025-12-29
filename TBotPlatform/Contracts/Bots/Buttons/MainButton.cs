using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots.Buttons;

public class MainButton(string button)
{
    private string _buttonValue;

    /// <summary>
    /// Тип кнопки
    /// </summary>
    public string ButtonName
    {
        get
        {
            return _buttonValue;
        }
        set
        {
            if (button.IsNull())
            {
                throw new ArgumentException("", button);
            }

            if (button.Length > ButtonsRuleConstant.ButtonsRuleNameLength)
            {
                _buttonValue = button[..ButtonsRuleConstant.ButtonsRuleNameLength];
            }

            _buttonValue = button;
        }
    }
}