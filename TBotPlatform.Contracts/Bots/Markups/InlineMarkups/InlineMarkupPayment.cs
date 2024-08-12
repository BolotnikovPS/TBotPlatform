using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupPayment(string buttonName)
    : InlineMarkupBase(buttonName)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);

        return InlineKeyboardButton.WithPayment(ButtonName);
    }
}