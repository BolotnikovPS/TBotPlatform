using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots.Buttons;

public class MainButtonList : List<MainButton>
{
    public new void Add(MainButton item)
    {
        if (SizeJsonCheck(item))
        {
            return;
        }

        base.Add(item);
    }

    private static bool SizeJsonCheck(MainButton item)
    {
        if (item.ButtonName.IsNull())
        {
            return false;
        }

        return item.ButtonName.Length > ButtonsRuleConstant.ButtonsRuleNameLength;
    }
}