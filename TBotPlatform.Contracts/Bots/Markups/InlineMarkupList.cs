using TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

namespace TBotPlatform.Contracts.Bots.Markups;

public sealed class InlineMarkupList : List<InlineMarkupBase>
{
    public new void Add(InlineMarkupBase item)
    {
        base.Add(item);
    }
}