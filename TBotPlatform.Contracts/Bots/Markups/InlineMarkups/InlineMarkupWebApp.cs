﻿using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupWebApp(string buttonName, string url)
    : InlineMarkupBase(buttonName)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        var webAppInfo = new WebAppInfo
        {
            Url = url,
        };

        return InlineKeyboardButton.WithWebApp(ButtonName, webAppInfo);
    }
}