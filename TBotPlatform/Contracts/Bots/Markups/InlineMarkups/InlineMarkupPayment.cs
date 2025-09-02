using TBotPlatform.Contracts.Bots.Markups.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupPayment(string buttonName)
    : InlineMarkupBase(buttonName, EInlineMarkupType.Payment)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);

        return InlineKeyboardButton.WithPay(ButtonName);
    }
}