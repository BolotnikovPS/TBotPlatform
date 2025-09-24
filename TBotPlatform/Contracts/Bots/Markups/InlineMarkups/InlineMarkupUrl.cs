using TBotPlatform.Contracts.Bots.Markups.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupUrl(string buttonName, string url) : InlineMarkupBase(buttonName, InlineMarkupType.Url)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        return InlineKeyboardButton.WithUrl(ButtonName, url);
    }
}