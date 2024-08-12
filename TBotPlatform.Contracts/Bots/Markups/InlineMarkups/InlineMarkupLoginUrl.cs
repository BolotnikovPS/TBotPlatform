using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupLoginUrl(string buttonName, LoginUrl loginUrl)
    : InlineMarkupBase(buttonName)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);

        return InlineKeyboardButton.WithLoginUrl(ButtonName, loginUrl);
    }
}