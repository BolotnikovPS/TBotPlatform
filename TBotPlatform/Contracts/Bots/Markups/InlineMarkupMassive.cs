namespace TBotPlatform.Contracts.Bots.Markups;

public sealed class InlineMarkupMassive
{
    /// <summary>
    /// Collection of inline buttons
    /// </summary>
    public InlineMarkupList InlineMarkups { get; set; } = null!;

    /// <summary>
    /// Number of buttons per row
    /// </summary>
    public int ButtonsPerRow { get; set; }
}