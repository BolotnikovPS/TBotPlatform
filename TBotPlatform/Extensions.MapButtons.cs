using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Extension;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform;

public static partial class Extensions
{
    public static ReplyKeyboardMarkup Map(this MainButtonMassiveList cakes) => GenerateButtons(cakes);

    public static InlineKeyboardButton[][] Map(this InlineMarkupMassiveList cakes) => [.. cakes.SelectMany(x => GenerateButtons(x.InlineMarkups, x.ButtonsPerRow))];

    public static ReplyKeyboardMarkup GenerateButtons(this MainButtonMassiveList cakes)
    {
        var result = cakes.Select(x => GenerateButtons(x.MainButtons)).ToArray();

        return new(result)
        {
            IsPersistent = cakes.ButtonsRule?.IsPersistent ?? false,
            ResizeKeyboard = cakes.ButtonsRule?.ResizeKeyboard ?? false,
            InputFieldPlaceholder = cakes.ButtonsRule?.InputFieldPlaceholder,
            OneTimeKeyboard = cakes.ButtonsRule?.OneTimeKeyboard ?? false,
            Selective = cakes.ButtonsRule?.Selective ?? false,
        };
    }

    public static IEnumerable<KeyboardButton> GenerateButtons(this MainButtonList cakes) => cakes.Select(q => new KeyboardButton(q.ButtonName));

    public static IEnumerable<InlineKeyboardButton[]> GenerateButtons(this InlineMarkupList cakes, int buttonsPerRow = 1)
        => cakes
        .Select(x => x.Format())
        .Where(z => z.IsNotNull())
        .Chunk(buttonsPerRow <= 0 ? 1 : buttonsPerRow)
        .Select(c => c.ToArray());
}
