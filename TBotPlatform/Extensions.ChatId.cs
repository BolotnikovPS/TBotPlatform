using TBotPlatform.Contracts.Bots.Exceptions;

namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Checks that the chatId is valid for Telegram API calls.
    /// </summary>
    /// <exception cref="ChatIdArgException">chatId equals 0, <see cref="long.MinValue"/> or <see cref="long.MaxValue"/>.</exception>
    public static void ThrowIfInvalidChatId(this long chatId)
    {
        if (chatId is 0 or long.MinValue or long.MaxValue)
        {
            throw new ChatIdArgException();
        }
    }
}
