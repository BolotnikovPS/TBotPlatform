#nullable enable
using TBotPlatform.Contracts.Bots.Markups.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupLoginUrl(
    string buttonName,
    string? url,
    string? forwardText,
    string? botUsername,
    bool requestWriteAccess
    ) : InlineMarkupBase(buttonName, EInlineMarkupType.LoginUrl)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        var loginUrl = new LoginUrl
        {
            Url = url,
            BotUsername = botUsername,
            ForwardText = forwardText,
            RequestWriteAccess = requestWriteAccess,
        };

        return InlineKeyboardButton.WithLoginUrl(ButtonName, loginUrl);
    }
}