#nullable enable
using TBotPlatform.Contracts.Bots.Markups.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupCallBackGame(string buttonName)
    : InlineMarkupBase(buttonName, EInlineMarkupType.CallbackGame)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);
        return InlineKeyboardButton.WithCallbackGame(ButtonName);
    }
}