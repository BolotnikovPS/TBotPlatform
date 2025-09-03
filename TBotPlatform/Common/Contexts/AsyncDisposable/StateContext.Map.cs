using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Extension;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    private static ReplyKeyboardMarkup Map(MainButtonMassiveList cakes)
        => GenerateButtons(cakes);

    private static InlineKeyboardButton[][] Map(InlineMarkupMassiveList cakes)
        => cakes.SelectMany(x => GenerateButtons(x.InlineMarkups, x.ButtonsPerRow)).ToArray();

    private static ReplyKeyboardMarkup GenerateButtons(MainButtonMassiveList cakes)
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

    private static IEnumerable<KeyboardButton> GenerateButtons(MainButtonList cakes)
    {
        return cakes
              .Select(q => new KeyboardButton(q.ButtonName))
              .ToArray();
    }

    private static IEnumerable<InlineKeyboardButton[]> GenerateButtons(InlineMarkupList cakes, int buttonsPerRow = 1)
    {
        return cakes
              .Select(x => x.Format())
              .Where(z => z.IsNotNull())
              .Chunk(buttonsPerRow <= 0 ? 1 : buttonsPerRow)
              .Select(c => c.ToArray())
              .ToList();
    }
}