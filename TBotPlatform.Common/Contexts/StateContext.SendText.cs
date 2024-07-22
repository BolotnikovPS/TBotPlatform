using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext
{
    public async Task<Message> SendTextMessageWithReplyAsync(string text, CancellationToken cancellationToken) 
        => await SendTextMessageAsync(text, true, cancellationToken);

    public Task<Message> SendTextMessageAsync(string text, CancellationToken cancellationToken) 
        => SendTextMessageAsync(text, false, cancellationToken);

    private Task<Message> SendTextMessageAsync(string text, bool withReply, CancellationToken cancellationToken)
    {
        if (UserDb.ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (text.IsNull())
        {
            return default;
        }

        if (text.Length > TextLength)
        {
            throw new TextLengthException(text.Length, TextLength);
        }

        var replyMarkup = !withReply
            ? null
            : new ForceReplyMarkup
            {
                Selective = true,
            };

        return botClient.SendTextMessageAsync(
            UserDb.ChatId,
            text,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );
    }

    public async Task SendLongTextMessageAsync(string text, CancellationToken cancellationToken)
    {
        if (UserDb.ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (text.IsNull())
        {
            return;
        }

        if (text.Length >= TextLength)
        {
            foreach (var tf in text.SplitByLength(TextLength))
            {
                await botClient.SendTextMessageAsync(
                    UserDb.ChatId,
                    tf,
                    cancellationToken: cancellationToken
                    );
            }

            return;
        }

        await botClient.SendTextMessageAsync(
            UserDb.ChatId,
            text,
            cancellationToken: cancellationToken
            );
    }
}