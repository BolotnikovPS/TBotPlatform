using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Extension;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext
{
    private static ReplyKeyboardMarkup Map(ButtonsRuleMassiveList cakes)
        => GenerateButtons(cakes);

    private static InlineKeyboardButton[][] Map(InlineMarkupMassiveList cakes)
        => GenerateButtons(cakes);

    private static ReplyKeyboardMarkup GenerateButtons(ButtonsRuleMassiveList cakes)
    {
        var result = cakes.Select(x => GenerateButtons(x.ButtonsRules)).ToArray();

        return new(result)
        {
            IsPersistent = true,
            ResizeKeyboard = true,
        };
    }

    private static IEnumerable<KeyboardButton> GenerateButtons(ButtonsRuleList cakes)
    {
        return cakes
              .Select(q => new KeyboardButton(q.Button))
              .ToArray();
    }

    private static InlineKeyboardButton[][] GenerateButtons(InlineMarkupMassiveList cakes)
    {
        return cakes.SelectMany(x => GenerateButtons(x.InlineMarkups, x.ButtonsPerRow)).ToArray();
    }

    private static IEnumerable<InlineKeyboardButton[]> GenerateButtons(InlineMarkupList cakes, int buttonsPerRow = 1)
    {
        if (buttonsPerRow <= 0)
        {
            buttonsPerRow = 1;
        }

        return cakes
              .Select(x => x.Format())
              .Where(z => z.IsNotNull())
              .Chunk(buttonsPerRow)
              .Select(c => c.ToArray())
              .ToList();
    }
}