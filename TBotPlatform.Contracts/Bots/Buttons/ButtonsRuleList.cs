using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots.Buttons;

public class ButtonsRuleList : List<ButtonsRule>
{
    public new void Add(ButtonsRule item)
    {
        if (SizeJsonCheck(item))
        {
            return;
        }

        base.Add(item);
    }

    private static bool SizeJsonCheck(ButtonsRule item)
    {
        if (item.ButtonName.IsNull())
        {
            return false;
        }

        return item.ButtonName.Length > ButtonsRuleConstant.ButtonsRuleNameLength;
    }
}