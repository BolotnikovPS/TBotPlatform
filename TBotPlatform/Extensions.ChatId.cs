using TBotPlatform.Contracts.Bots.Exceptions;

namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Проверяет, что chatId пригоден для вызовов Telegram API.
    /// </summary>
    /// <exception cref="ChatIdArgException">chatId равен 0, <see cref="long.MinValue"/> или <see cref="long.MaxValue"/>.</exception>
    public static void ThrowIfInvalidChatId(this long chatId)
    {
        if (chatId is 0 or long.MinValue or long.MaxValue)
        {
            throw new ChatIdArgException();
        }
    }
}
