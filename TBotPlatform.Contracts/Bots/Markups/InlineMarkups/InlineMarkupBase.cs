using TBotPlatform.Contracts.Bots.Constant;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public abstract class InlineMarkupBase(string buttonName)
{
    /// <summary>
    /// Название inline кнопки
    /// </summary>
    protected string ButtonName { get; } = buttonName.Length > InlineMarkupConstant.ButtonNameLength
        ? buttonName[..InlineMarkupConstant.ButtonNameLength]
        : buttonName;

    public abstract InlineKeyboardButton Format();
}