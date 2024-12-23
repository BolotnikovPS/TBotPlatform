﻿using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    public Task<ChatResult> SendTextMessageWithReplyAsync(string text, bool disableNotification, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, withReply: true, disableNotification, cancellationToken);

    public Task<ChatResult> SendTextMessageWithReplyAsync(string text, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, withReply: true, disableNotification: false, cancellationToken);

    public Task<ChatResult> SendTextMessageAsync(string text, bool disableNotification, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, withReply: false, disableNotification, cancellationToken);

    public Task<ChatResult> SendTextMessageAsync(string text, CancellationToken cancellationToken)
        => SendTextMessageAsync(text, withReply: false, disableNotification: false, cancellationToken);

    private async Task<ChatResult> SendTextMessageAsync(string text, bool withReply, bool disableNotification, CancellationToken cancellationToken)
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

        var result = await telegramContext.SendTextMessageAsync(
            ChatId,
            text,
            replyMarkup,
            disableNotification,
            cancellationToken
            );

        return telegramMapping.MessageToResult(result);
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
                await telegramContext.SendTextMessageAsync(ChatId, tf, disableNotification, cancellationToken);
            }

            return;
        }

        await telegramContext.SendTextMessageAsync(ChatId, text, disableNotification, cancellationToken);
    }

    public Task SendLongTextMessageAsync(string text, CancellationToken cancellationToken)
        => SendLongTextMessageAsync(text, disableNotification: false, cancellationToken);
}