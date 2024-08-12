using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupSwitchInlineQuery(string buttonName, string query)
    : InlineMarkupBase(buttonName)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        return InlineKeyboardButton.WithSwitchInlineQuery(ButtonName, query);
    }
}