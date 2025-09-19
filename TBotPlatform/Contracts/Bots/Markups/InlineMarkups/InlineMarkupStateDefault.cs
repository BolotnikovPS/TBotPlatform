using TBotPlatform.Contracts.Bots.Markups.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupStateDefault(string buttonName) : InlineMarkupBase(buttonName, InlineMarkupType.None)
{
    public override InlineKeyboardButton Format() => new(ButtonName);
}