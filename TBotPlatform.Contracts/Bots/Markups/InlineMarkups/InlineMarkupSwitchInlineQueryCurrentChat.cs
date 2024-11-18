using TBotPlatform.Contracts.Bots.Markups.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupSwitchInlineQueryCurrentChat(string buttonName, string query)
    : InlineMarkupBase(buttonName, EInlineMarkupType.SwitchInlineQueryCurrentChat)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        return InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(ButtonName, query);
    }
}