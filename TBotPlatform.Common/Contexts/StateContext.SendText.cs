using TBotPlatform.Contracts.Bots.Constant;
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
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (text.IsNull())
        {
            return default;
        }

        if (text.Length > StateContextConstant.TextLength)
        {
            throw new TextLengthException(text.Length, StateContextConstant.TextLength);
        }

        var replyMarkup = !withReply
            ? null
            : new ForceReplyMarkup
            {
                Selective = true,
            };

        return botClient.SendTextMessageAsync(
            ChatId,
            text,
            replyMarkup,
            cancellationToken
            );
    }

    public async Task SendLongTextMessageAsync(string text, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (text.IsNull())
        {
            return;
        }

        if (text.Length >= StateContextConstant.TextLength)
        {
            foreach (var tf in text.SplitByLength(StateContextConstant.TextLength))
            {
                await botClient.SendTextMessageAsync(
                    ChatId,
                    tf,
                    cancellationToken
                    );
            }

            return;
        }

        await botClient.SendTextMessageAsync(
            ChatId,
            text,
            cancellationToken
            );
    }
}