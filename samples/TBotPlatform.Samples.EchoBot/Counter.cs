using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Demonstrates <c>IStateContext.UpdateInlineMarkup</c>: only the keyboard of the message is updated,
/// its text and its position in the chat stay unchanged.
/// </summary>
internal static class Counter
{
    /// <summary>
    /// Counter keyboard: a press is handled by the <see cref="CounterClickState"/> state
    /// </summary>
    public static InlineMarkupMassiveList CreateMarkup(int count)
        => new()
        {
            new InlineMarkupMassive
            {
                InlineMarkups =
                [
                    new InlineMarkupState($"Нажатий: {count}", nameof(CounterClickState), (count + 1).ToString()),
                ],
                ButtonsPerRow = 1,
            },
        };
}
