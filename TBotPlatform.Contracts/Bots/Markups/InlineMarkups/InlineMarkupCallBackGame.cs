#nullable enable
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupCallBackGame(string buttonName, CallbackGame? callbackGame = default)
    : InlineMarkupBase(buttonName)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);

        return InlineKeyboardButton.WithCallBackGame(ButtonName, callbackGame);
    }
}