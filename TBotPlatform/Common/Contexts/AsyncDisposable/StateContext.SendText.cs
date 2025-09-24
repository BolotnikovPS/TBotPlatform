using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public Task<Message> SendTextMessageWithReply(string text, bool disableNotification, CancellationToken cancellationToken)
        => SendTextMessage(text, withReply: true, disableNotification, cancellationToken);

    public Task<Message> SendTextMessageWithReply(string text, CancellationToken cancellationToken)
        => SendTextMessage(text, withReply: true, disableNotification: false, cancellationToken);

    public Task<Message> SendTextMessage(string text, bool disableNotification, CancellationToken cancellationToken)
        => SendTextMessage(text, withReply: false, disableNotification, cancellationToken);

    public Task<Message> SendTextMessage(string text, CancellationToken cancellationToken)
        => SendTextMessage(text, withReply: false, disableNotification: false, cancellationToken);

    private Task<Message> SendTextMessage(string text, bool withReply, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        if (text.IsNull())
        {
            return null;
        }

        TextLengthValidOrThrow(text);

        var replyMarkup = !withReply
            ? null
            : new ForceReplyMarkup
            {
                Selective = true,
            };

        return telegramContext.SendMessage(
            chatId,
            text,
            ParseMode,
            replyMarkup: replyMarkup,
            disableNotification: disableNotification,
            cancellationToken: cancellationToken
            );
    }

    public async Task SendLongTextMessage(string text, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        if (text.IsNull())
        {
            return;
        }

        if (text.Length >= StateContextConstant.TextLength)
        {
            foreach (var tf in text.SplitByLength(StateContextConstant.TextLength))
            {
                await telegramContext.SendMessage(
                    chatId,
                    tf,
                    disableNotification: disableNotification,
                    cancellationToken: cancellationToken
                    );
            }

            return;
        }

        await telegramContext.SendMessage(
            chatId,
            text,
            ParseMode,
            disableNotification: disableNotification,
            cancellationToken: cancellationToken
            );
    }

    public Task SendLongTextMessage(string text, CancellationToken cancellationToken)
        => SendLongTextMessage(text, disableNotification: false, cancellationToken);
}