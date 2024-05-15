namespace TBotPlatform.Contracts.Bots.Markups;

public sealed class InlineMarkupMassivList : List<InlineMarkupMassiv>
{
    public new void Add(InlineMarkupMassiv item)
    {
        if (item.ButtonsPerRow <= 0)
        {
            item.ButtonsPerRow = 1;
        }

        base.Add(item);
    }
}