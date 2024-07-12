using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext<T>
{
    public async Task<Message> SendTextMessageWithReplyAsync(
        string text,
        CancellationToken cancellationToken
        ) => await SendTextMessageAsync(
        text,
        true,
        cancellationToken
        );

    public Task<Message> SendTextMessageAsync(
        string text,
        CancellationToken cancellationToken
        ) => SendTextMessageAsync(
        text,
        false,
        cancellationToken
        );

    private async Task<Message> SendTextMessageAsync(
        string text,
        bool withReply,
        CancellationToken cancellationToken
        )
    {
        if (!UserDb.ChatId.CheckAny())
        {
            throw new ChatIdArgException();
        }

        if (!text.CheckAny())
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

        var task = botClient.SendTextMessageAsync(
            UserDb.ChatId,
            text,
            parseMode: ParseMode,
            protectContent: ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return await ExecuteTaskAsync(task, cancellationToken);
    }

    public async Task SendLongTextMessageAsync(
        string text,
        CancellationToken cancellationToken
        )
    {
        if (!UserDb.ChatId.CheckAny())
        {
            throw new ChatIdArgException();
        }

        if (!text.CheckAny())
        {
            return;
        }

        if (text.Length >= TextLength)
        {
            foreach (var tf in text.SplitByLength(TextLength))
            {
                var taskText = botClient.SendTextMessageAsync(
                    UserDb.ChatId,
                    tf,
                    parseMode: ParseMode,
                    protectContent: ProtectContent,
                    cancellationToken: cancellationToken
                    );

                await ExecuteTaskAsync(taskText, cancellationToken);
            }

            return;
        }

        var task = botClient.SendTextMessageAsync(
            UserDb.ChatId,
            text,
            parseMode: ParseMode,
            protectContent: ProtectContent,
            cancellationToken: cancellationToken
            );

        await ExecuteTaskAsync(task, cancellationToken);
    }
}