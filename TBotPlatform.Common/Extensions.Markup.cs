using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.Markups.InlineMarkups;
using TBotPlatform.Extension;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common;

public static partial class Extensions
{
    public static InlineMarkupList GetInlineMarkupList(this InlineKeyboardMarkup replyMarkup)
    {
        if (replyMarkup.IsNull())
        {
            return default;
        }

        var result = new InlineMarkupList();

        var inlineMarkups = replyMarkup.InlineKeyboard.SelectMany(x => x.Select(GetInlineMarkup));

        foreach (var inlineMarkup in inlineMarkups)
        {
            result.Add(inlineMarkup);
        }

        return result;
    }

    public static InlineMarkupBase GetInlineMarkup(this InlineKeyboardButton button)
    {
        if (button.CallbackData.IsNotNull())
        {
            if (button.CallbackData!.TryParseJson<MarkupNextState>(out var newMarkupNextState))
            {
                return new InlineMarkupState(button.Text, newMarkupNextState);
            }
        }

        if (button.Url.IsNotNull())
        {
            return new InlineMarkupUrl(button.Text, button.Url);
        }

        if (button.LoginUrl.IsNotNull())
        {
            var loginUrl = button.LoginUrl;
            return new InlineMarkupLoginUrl(
                button.Text,
                loginUrl?.Url,
                loginUrl?.ForwardText,
                loginUrl?.BotUsername,
                loginUrl?.RequestWriteAccess ?? false
                );
        }

        if (button.CallbackGame.IsNotNull())
        {
            return new InlineMarkupCallBackGame(button.Text);
        }

        if (button.Pay)
        {
            return new InlineMarkupPayment(button.Text);
        }

        if (button.SwitchInlineQuery.IsNotNull())
        {
            return new InlineMarkupSwitchInlineQuery(button.Text, button.SwitchInlineQuery);
        }

        if (button.SwitchInlineQueryChosenChat.IsNotNull())
        {
            var switchInlineQueryChosenChat = button.SwitchInlineQueryChosenChat;
            return new InlineMarkupSwitchInlineQueryChosenChat(
                button.Text,
                switchInlineQueryChosenChat?.Query,
                switchInlineQueryChosenChat?.AllowUserChats ?? false,
                switchInlineQueryChosenChat?.AllowBotChats ?? false,
                switchInlineQueryChosenChat?.AllowGroupChats ?? false,
                switchInlineQueryChosenChat?.AllowChannelChats ?? false
                );
        }

        if (button.SwitchInlineQueryCurrentChat.IsNotNull())
        {
            return new InlineMarkupSwitchInlineQueryCurrentChat(button.Text, button.SwitchInlineQueryCurrentChat);
        }

        if (button.WebApp.IsNotNull())
        {
            return new InlineMarkupWebApp(button.Text, button.WebApp!.Url);
        }

        if (button.CopyText.IsNotNull())
        {
            return new InlineMarkupCopyText(button.Text, button.CopyText?.Text);
        }

        return new InlineMarkupStateDefault(button.Text);
    }
}