namespace TBotPlatform.Contracts.Bots.Markups;

public sealed class InlineMarkupMassiveList : List<InlineMarkupMassive>
{
    public new void Add(InlineMarkupMassive item)
    {
        if (item.ButtonsPerRow <= 0)
        {
            item.ButtonsPerRow = 1;
        }

        base.Add(item);
    }
}