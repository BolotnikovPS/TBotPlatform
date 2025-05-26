#nullable enable
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    private void ChatIdValidOrThrow()
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }
    }

    internal static void ChatIdValidOrThrow(long chatId)
    {
        if (chatId.IsDefault())
        {
            throw new ChatIdArgException();
        }
    }

    private static void TextLengthValidOrThrow(string? text)
    {
        if (text?.Length > StateContextConstant.TextLength)
        {
            throw new TextLengthException(text.Length, StateContextConstant.TextLength);
        }
    }

    private static void CallbackQueryValidOrThrow(CallbackQuery? callbackQueryOrNull)
    {
        if (callbackQueryOrNull.IsNull())
        {
            throw new CallbackQueryArgException();
        }
    }

    public static void MessageIdValidOrThrow(long messageId)
    {
        if (messageId.IsNull())
        {
            throw new MessageIdArgException();
        }
    }
}