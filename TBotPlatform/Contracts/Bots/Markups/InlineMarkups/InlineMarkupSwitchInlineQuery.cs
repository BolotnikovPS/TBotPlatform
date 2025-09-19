using TBotPlatform.Contracts.Bots.Markups.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupSwitchInlineQuery(string buttonName, string query) : InlineMarkupBase(buttonName, InlineMarkupType.SwitchInlineQuery)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        return InlineKeyboardButton.WithSwitchInlineQuery(ButtonName, query);
    }
}