using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupSwitchInlineQueryChosenChat(string buttonName, SwitchInlineQueryChosenChat switchInlineQueryChosenChat)
    : InlineMarkupBase(buttonName)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);

        return InlineKeyboardButton.WithSwitchInlineQueryChosenChat(ButtonName, switchInlineQueryChosenChat);
    }
}