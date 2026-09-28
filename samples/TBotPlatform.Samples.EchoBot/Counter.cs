using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Демонстрация <c>IStateContext.UpdateInlineMarkup</c>: у сообщения обновляется только клавиатура,
/// текст и позиция сообщения в чате не меняются.
/// </summary>
internal static class Counter
{
    /// <summary>
    /// Клавиатура счётчика: нажатие обрабатывает состояние <see cref="CounterClickState"/>
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
