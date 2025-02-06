#nullable enable
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.Markups.InlineMarkups;
using TBotPlatform.Extension;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common;

public static partial class Extensions
{
    public static bool TryGetInlineMarkupList(this InlineKeyboardMarkup replyMarkup, out InlineMarkupList? inlineMarkupList)
    {
        inlineMarkupList = null;
        if (replyMarkup.IsNull())
        {
            return false;
        }

        inlineMarkupList = [];

        var inlineMarkups = replyMarkup.InlineKeyboard
                                       .SelectMany(x => x.Select(z => z.TryGetInlineMarkup(out var e) ? e : null))
                                       .Where(z => z.IsNotNull());

        foreach (var inlineMarkup in inlineMarkups)
        {
            inlineMarkupList.Add(inlineMarkup);
        }

        return true;
    }

    public static bool TryGetInlineMarkup(this InlineKeyboardButton button, out InlineMarkupBase? inlineMarkupBase)
    {
        inlineMarkupBase = null;
        if (button.CallbackData.IsNotNull())
        {
            if (button.CallbackData!.TryParseJson<MarkupNextState>(out var newMarkupNextState))
            {
                inlineMarkupBase = new InlineMarkupState(button.Text, newMarkupNextState);
                return true;
            }
        }

        if (button.Url.IsNotNull())
        {
            inlineMarkupBase = new InlineMarkupUrl(button.Text, button.Url);
            return true;
        }

        if (button.LoginUrl.IsNotNull())
        {
            var loginUrl = button.LoginUrl;
            inlineMarkupBase = new InlineMarkupLoginUrl(
                button.Text,
                loginUrl?.Url,
                loginUrl?.ForwardText,
                loginUrl?.BotUsername,
                loginUrl?.RequestWriteAccess ?? false
                );
            return true;
        }

        if (button.CallbackGame.IsNotNull())
        {
            inlineMarkupBase = new InlineMarkupCallBackGame(button.Text);
            return true;
        }

        if (button.Pay)
        {
            inlineMarkupBase = new InlineMarkupPayment(button.Text);
            return true;
        }

        if (button.SwitchInlineQuery.IsNotNull())
        {
            inlineMarkupBase = new InlineMarkupSwitchInlineQuery(button.Text, button.SwitchInlineQuery);
            return true;
        }

        if (button.SwitchInlineQueryChosenChat.IsNotNull())
        {
            var switchInlineQueryChosenChat = button.SwitchInlineQueryChosenChat;
            inlineMarkupBase = new InlineMarkupSwitchInlineQueryChosenChat(
                button.Text,
                switchInlineQueryChosenChat?.Query,
                switchInlineQueryChosenChat?.AllowUserChats ?? false,
                switchInlineQueryChosenChat?.AllowBotChats ?? false,
                switchInlineQueryChosenChat?.AllowGroupChats ?? false,
                switchInlineQueryChosenChat?.AllowChannelChats ?? false
                );
            return true;
        }

        if (button.SwitchInlineQueryCurrentChat.IsNotNull())
        {
            inlineMarkupBase = new InlineMarkupSwitchInlineQueryCurrentChat(button.Text, button.SwitchInlineQueryCurrentChat);
            return true;
        }

        if (button.WebApp.IsNotNull())
        {
            inlineMarkupBase = new InlineMarkupWebApp(button.Text, button.WebApp!.Url);
            return true;
        }

        if (button.CopyText.IsNotNull())
        {
            inlineMarkupBase = new InlineMarkupCopyText(button.Text, button.CopyText?.Text);
            return true;
        }

        if (button.Text.IsNull())
        {
            return false;
        }

        inlineMarkupBase = new InlineMarkupStateDefault(button.Text);
        return true;
    }
}