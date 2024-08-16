﻿using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public Task<ChatResult> SendTextMessageWithReplyAsync(string text, bool disableNotification, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, true, disableNotification, cancellationToken);

    public Task<ChatResult> SendTextMessageWithReplyAsync(string text, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, true, false, cancellationToken);

    public Task<ChatResult> SendTextMessageAsync(string text, bool disableNotification, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, false, disableNotification, cancellationToken);

    public Task<ChatResult> SendTextMessageAsync(string text, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, false, false, cancellationToken);

    private async Task<ChatResult> SendTextMessageAsync(string text, bool withReply, bool disableNotification, CancellationToken cancellationToken)
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

        var result = await botClient.SendTextMessageAsync(
            ChatId,
            text,
            replyMarkup,
            disableNotification,
            cancellationToken
            );

        return telegramMapping.MappingMessage(result);
    }

    public async Task SendLongTextMessageAsync(string text, bool disableNotification, CancellationToken cancellationToken)
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
                    disableNotification,
                    cancellationToken
                    );
            }

            return;
        }

        await botClient.SendTextMessageAsync(
            ChatId,
            text,
            disableNotification,
            cancellationToken
            );
    }

    public Task SendLongTextMessageAsync(string text, CancellationToken cancellationToken)
        => SendLongTextMessageAsync(text, false, cancellationToken);
}