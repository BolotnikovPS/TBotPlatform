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

    public Task<Message> SendTextMessage(
        string text,
        bool disableNotification,
        ReplyParameters? replyParameters,
        LinkPreviewOptions? linkPreviewOptions,
        string? messageEffectId,
        CancellationToken cancellationToken
        )
    {
        ChatIdValidOrThrow();

        if (text.IsNull())
        {
            // Сообщение не отправляется, но возвращать null вместо Task нельзя — вызывающий получит NRE на await.
            return Task.FromResult<Message>(null!);
        }

        TextLengthValidOrThrow(text);

        return telegramContext.SendMessage(
            chatId,
            text,
            ParseMode,
            replyParameters: replyParameters,
            linkPreviewOptions: linkPreviewOptions,
            disableNotification: disableNotification,
            messageEffectId: messageEffectId,
            cancellationToken: cancellationToken
            );
    }

    public Task<Message> SendTextMessageWithEntities(string text, IReadOnlyList<MessageEntity> entities, CancellationToken cancellationToken)
        => SendTextMessageWithEntities(text, entities, disableNotification: false, cancellationToken);

    public Task<Message> SendTextMessageWithEntities(
        string text,
        IReadOnlyList<MessageEntity> entities,
        bool disableNotification,
        CancellationToken cancellationToken
        )
    {
        ChatIdValidOrThrow();

        if (text.IsNull() || entities.IsNull() || entities.Count == 0)
        {
            // Сообщение не отправляется, но возвращать null вместо Task нельзя — вызывающий получит NRE на await.
            return Task.FromResult<Message>(null!);
        }

        TextLengthValidOrThrow(text);

        // Разметка передается сущностями, поэтому parse mode отключен: Telegram не принимает их одновременно.
        return telegramContext.SendMessage(
            chatId,
            text,
            parseMode: default,
            entities: entities,
            disableNotification: disableNotification,
            cancellationToken: cancellationToken
            );
    }

    private Task<Message> SendTextMessage(string text, bool withReply, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        if (text.IsNull())
        {
            // Сообщение не отправляется, но возвращать null вместо Task нельзя — вызывающий получит NRE на await.
            return Task.FromResult<Message>(null!);
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
                    ParseMode,
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