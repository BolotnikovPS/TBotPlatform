using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;
using PMode = Telegram.Bot.Types.Enums.ParseMode;

namespace TBotPlatform.Common.Contexts;

internal class TelegramContext(
    ILogger<TelegramContext> logger,
    TelegramSettings telegramSettings,
    ITelegramBotClient botClient
    ) : ITelegramContext
{
    private static readonly TelegramContextTaskQueue Queue = new();

    private const PMode ParseMode = PMode.Html;

    public Task DeleteMessageAsync(
        long chatId,
        int messageId,
        CancellationToken cancellationToken
        )
    {
        var task = botClient.DeleteMessageAsync(
            chatId,
            messageId,
            cancellationToken
            );

        return ExecuteEnqueueSafety(task, cancellationToken);
    }


    public Task<Message> EditMessageTextAsync(
        long chatId,
        int messageId,
        string text,
        InlineKeyboardMarkup replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var task = botClient.EditMessageTextAsync(
            chatId,
            messageId,
            text,
            ParseMode,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, cancellationToken);
    }

    public Task<Message> EditMessageTextAsync(
        long chatId,
        int messageId,
        string text,
        CancellationToken cancellationToken
        )
        => EditMessageTextAsync(
            chatId,
            messageId,
            text,
            null,
            cancellationToken
            );

    public Task<Message> EditMessageCaptionAsync(
        long chatId,
        int messageId,
        string caption,
        InlineKeyboardMarkup replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var task = botClient.EditMessageCaptionAsync(
            chatId,
            messageId,
            caption,
            ParseMode,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, cancellationToken);
    }

    public Task<Message> EditMessageCaptionAsync(
        long chatId,
        int messageId,
        string caption,
        CancellationToken cancellationToken
        )
        => EditMessageCaptionAsync(
            chatId,
            messageId,
            caption,
            null,
            cancellationToken
            );

    public Task<Message> SendTextMessageAsync(
        long chatId,
        string text,
        IReplyMarkup replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var task = botClient.SendTextMessageAsync(
            chatId,
            text,
            parseMode: ParseMode,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );
        return ExecuteEnqueueSafety(task, cancellationToken);
    }

    public Task<Message> SendTextMessageAsync(
        long chatId,
        string text,
        CancellationToken cancellationToken
        )
        => SendTextMessageAsync(
            chatId,
            text,
            null,
            cancellationToken
            );

    public Task SendChatActionAsync(
        long chatId,
        ChatAction chatAction,
        CancellationToken cancellationToken
        )
    {
        var task = botClient.SendChatActionAsync(
            chatId,
            chatAction,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, cancellationToken);
    }

    public Task<Message> SendDocumentAsync(
        long chatId,
        InputFile document,
        IReplyMarkup replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var task = botClient.SendDocumentAsync(
            chatId,
            document,
            parseMode: ParseMode,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, cancellationToken);
    }

    public Task<Message> SendDocumentAsync(
        long chatId,
        InputFile document,
        CancellationToken cancellationToken
        )
        => SendDocumentAsync(
            chatId,
            document,
            null,
            cancellationToken
            );

    public Task<Message> SendPhotoAsync(
        long chatId,
        InputFile photo,
        string caption,
        IReplyMarkup replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var task = botClient.SendPhotoAsync(
            chatId,
            photo,
            caption: caption,
            parseMode: ParseMode,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, cancellationToken);
    }

    public Task<Message> SendPhotoAsync(
        long chatId,
        InputFile photo,
        CancellationToken cancellationToken
        )
        => SendPhotoAsync(
            chatId,
            photo,
            null,
            null,
            cancellationToken
            );

    public Task DownloadFileAsync(
        string filePath,
        Stream destination,
        CancellationToken cancellationToken
        )
    {
        var task = botClient.DownloadFileAsync(
            filePath,
            destination,
            cancellationToken
            );

        return ExecuteEnqueueSafety(task, cancellationToken);
    }

    public Task<File> GetFileAsync(
        string fileId,
        CancellationToken cancellationToken
        )
    {
        var task = botClient.GetFileAsync(
            fileId,
            cancellationToken
            );

        return ExecuteEnqueueSafety(task, cancellationToken);
    }
    
    private async Task ExecuteEnqueueSafety(Task method, CancellationToken cancellationToken)
    {
        try
        {
            await Queue.Enqueue(() => method, cancellationToken);
            return;
        }
        catch (ApiRequestException ex)
        {
            var delay = ex.Parameters.IsNotNull()
                        && ex!.Parameters!.RetryAfter.HasValue
                ? ex.Parameters.RetryAfter.Value
                : 100;

            logger.LogError(ex, "Ошибка. Задержка {delay}.", delay);

            await Task.Delay(delay, cancellationToken);
        }

        await Queue.Enqueue(() => method, cancellationToken);
    }

    private async Task<T> ExecuteEnqueueSafety<T>(Task<T> method, CancellationToken cancellationToken)
    {
        try
        {
            return await Queue.Enqueue(() => method, cancellationToken);
        }
        catch (ApiRequestException ex)
        {
            var delay = ex.Parameters.IsNotNull()
                        && ex!.Parameters!.RetryAfter.HasValue
                ? ex.Parameters.RetryAfter.Value
                : 100;

            logger.LogError(ex, "Ошибка. Задержка {delay}.", delay);

            await Task.Delay(delay, cancellationToken);
        }

        return await Queue.Enqueue(() => method, cancellationToken);
    }
}