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
        if (item.Button.IsNull())
        {
            return false;
        }

        return item.Button.Length > 64;
    }
}