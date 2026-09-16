namespace TBotPlatform.Contracts.Bots.Markups;

public sealed class InlineMarkupMassive
{
    /// <summary>
    /// Коллекция кнопок inline
    /// </summary>
    public InlineMarkupList InlineMarkups { get; set; } = null!;

    /// <summary>
    /// Число кнопок в строке
    /// </summary>
    public int ButtonsPerRow { get; set; }
}