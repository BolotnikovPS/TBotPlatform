using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Markups.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

/// <summary>
/// Creates an inline button <see cref="InlineKeyboardButton"/>
/// </summary>
/// <param name="buttonName">Button name</param>
/// <param name="type">Inline button type</param>
public abstract class InlineMarkupBase(string buttonName, InlineMarkupType type)
{
    protected readonly InlineMarkupType Type = type;

    /// <summary>
    /// Inline button name
    /// </summary>
    protected string ButtonName { get; } = buttonName.Length > InlineMarkupConstant.ButtonNameLength
        ? buttonName[..InlineMarkupConstant.ButtonNameLength]
        : buttonName;

    public abstract InlineKeyboardButton Format();
}