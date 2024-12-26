#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    internal void ChatIdValidOrThrow(long chatId)
    {
        if (chatId.IsDefault())
        {
            throw new ChatIdArgException();
        }
    }

    private void ChatIdValidOrThrow()
    {
        if (ChatId.IsDefault())
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

    private static void CallbackQueryValidOrThrow(ChatCallbackQuery? callbackQueryOrNull)
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