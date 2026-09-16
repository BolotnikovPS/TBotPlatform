namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Проверяет, что chatId пригоден для вызовов Telegram API.
    /// </summary>
    public static void ThrowIfInvalidChatId(this long chatId)
    {
        if (chatId is 0 or long.MinValue or long.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(chatId), chatId, "Некорректный chatId.");
        }
    }
}
