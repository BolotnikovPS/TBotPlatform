using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupUrl(string buttonName, string url)
    : InlineMarkupBase(buttonName)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        return InlineKeyboardButton.WithUrl(ButtonName, url);
    }
}