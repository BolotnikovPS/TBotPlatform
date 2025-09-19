#nullable enable
using TBotPlatform.Contracts.Bots.Markups.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public class InlineMarkupSwitchInlineQueryChosenChat(
    string buttonName,
    string? query,
    bool allowUserChats,
    bool allowBotChats,
    bool allowGroupChats,
    bool allowChannelChats
    ) : InlineMarkupBase(buttonName, InlineMarkupType.SwitchInlineQueryChosenChat)
{
    public override InlineKeyboardButton Format()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ButtonName);

        var switchInlineQueryChosenChat = new SwitchInlineQueryChosenChat
        {
            Query = query,
            AllowUserChats = allowUserChats,
            AllowBotChats = allowBotChats,
            AllowGroupChats = allowGroupChats,
            AllowChannelChats = allowChannelChats,
        };

        return InlineKeyboardButton.WithSwitchInlineQueryChosenChat(ButtonName, switchInlineQueryChosenChat);
    }
}