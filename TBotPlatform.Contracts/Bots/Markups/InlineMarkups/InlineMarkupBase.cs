using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Bots.Markups.InlineMarkups;

public abstract class InlineMarkupBase(string buttonName)
{
    /// <summary>
    /// Название inline кнопки
    /// </summary>
    protected string ButtonName { get; set; } = buttonName;

    public abstract InlineKeyboardButton Format();
}