using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Markups.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

/// <summary>
/// Создание inline кнопки <see cref="InlineKeyboardButton"/>
/// </summary>
/// <param name="buttonName"></param>
public abstract class InlineMarkupBase(string buttonName, EInlineMarkupType type)
{
    protected readonly EInlineMarkupType Type = type;

    /// <summary>
    /// Название inline кнопки
    /// </summary>
    protected string ButtonName { get; } = buttonName.Length > InlineMarkupConstant.ButtonNameLength
        ? buttonName[..InlineMarkupConstant.ButtonNameLength]
        : buttonName;

    public abstract InlineKeyboardButton Format();
}