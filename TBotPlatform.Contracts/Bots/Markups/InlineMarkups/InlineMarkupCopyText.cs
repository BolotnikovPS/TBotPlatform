using TBotPlatform.Contracts.Bots.Markups.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupCopyText(string buttonName, string copyText)
    : InlineMarkupBase(buttonName, EInlineMarkupType.CopyText)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);
        ArgumentException.ThrowIfNullOrWhiteSpace(copyText);

        var copyTextButton = new CopyTextButton
        {
            Text = copyText,
        };

        return InlineKeyboardButton.WithCopyText(ButtonName, copyTextButton);
    }
}