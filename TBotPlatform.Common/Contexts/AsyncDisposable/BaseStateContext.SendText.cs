using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    public Task<Message> SendTextMessageWithReplyAsync(string text, bool disableNotification, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, withReply: true, disableNotification, cancellationToken);

    public Task<Message> SendTextMessageWithReplyAsync(string text, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, withReply: true, disableNotification: false, cancellationToken);

    public Task<Message> SendTextMessageAsync(string text, bool disableNotification, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, withReply: false, disableNotification, cancellationToken);

    public Task<Message> SendTextMessageAsync(string text, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, withReply: false, disableNotification: false, cancellationToken);

    private Task<Message> SendTextMessageAsync(string text, bool withReply, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        if (text.IsNull())
        {
            return default;
        }

        TextLengthValidOrThrow(text);

        var replyMarkup = !withReply
            ? null
            : new ForceReplyMarkup
            {
                Selective = true,
            };

        return telegramContext.SendMessage(
            ChatId,
            text,
            ParseMode,
            replyMarkup: replyMarkup,
            disableNotification: disableNotification,
            cancellationToken: cancellationToken
            );
    }

    public async Task SendLongTextMessageAsync(string text, bool disableNotification, CancellationToken cancellationToken)
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
                    ChatId,
                    tf,
                    disableNotification: disableNotification,
                    cancellationToken: cancellationToken
                    );
            }

            return;
        }

        await telegramContext.SendMessage(
            ChatId,
            text,
            ParseMode,
            disableNotification: disableNotification,
            cancellationToken: cancellationToken
            );
    }

    public Task SendLongTextMessageAsync(string text, CancellationToken cancellationToken)
        => SendLongTextMessageAsync(text, disableNotification: false, cancellationToken);
}