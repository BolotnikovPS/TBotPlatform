using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;
using PMode = Telegram.Bot.Types.Enums.ParseMode;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext(
    ILogger<TelegramContext> logger,
    ITelegramBotClient botClient,
    TelegramSettings telegramSettings,
    ITelegramStatisticContext telegramStatisticContext
    ) : ITelegramContext
{
    private const PMode ParseMode = PMode.Html;

    public Task DeleteMessageAsync(long chatId, int messageId, CancellationToken cancellationToken)
    {
        var statistic = new StatisticMessage
        {
            ChatId = chatId,
            MessageId = messageId,
            OperationType = nameof(DeleteMessageAsync),
        };

        var task = botClient.DeleteMessageAsync(
            chatId,
            messageId,
            cancellationToken
            );

        return ExecuteEnqueueSafety(task, statistic, cancellationToken);
    }

    public Task<Message> EditMessageTextAsync(
        long chatId,
        int messageId,
        string text,
        InlineKeyboardMarkup replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var statistic = new StatisticMessage
        {
            ChatId = chatId,
            MessageId = messageId,
            Message = text,
            InlineKeyboardMarkup = replyMarkup,
            OperationType = nameof(EditMessageTextAsync),
        };

        var task = botClient.EditMessageTextAsync(
            chatId,
            messageId,
            text,
            ParseMode,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, statistic, cancellationToken);
    }

    public Task<Message> EditMessageTextAsync(long chatId, int messageId, string text, CancellationToken cancellationToken)
        => EditMessageTextAsync(
            chatId,
            messageId,
            text,
            null,
            cancellationToken
            );

    public Task<Message> EditMessageCaptionAsync(long chatId, int messageId, string caption, InlineKeyboardMarkup replyMarkup, CancellationToken cancellationToken)
    {
        var statistic = new StatisticMessage
        {
            ChatId = chatId,
            MessageId = messageId,
            InlineKeyboardMarkup = replyMarkup,
            Caption = caption,
            OperationType = nameof(EditMessageCaptionAsync),
        };

        var task = botClient.EditMessageCaptionAsync(
            chatId,
            messageId,
            caption,
            ParseMode,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, statistic, cancellationToken);
    }

    public Task<Message> EditMessageCaptionAsync(long chatId, int messageId, string caption, CancellationToken cancellationToken)
        => EditMessageCaptionAsync(
            chatId,
            messageId,
            caption,
            null,
            cancellationToken
            );

    public Task<Message> SendTextMessageAsync(long chatId, string text, IReplyMarkup replyMarkup, CancellationToken cancellationToken)
    {
        var statistic = new StatisticMessage
        {
            ChatId = chatId,
            Message = text,
            ReplyMarkup = replyMarkup,
            OperationType = nameof(SendTextMessageAsync),
        };

        var task = botClient.SendTextMessageAsync(
            chatId,
            text,
            parseMode: ParseMode,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, statistic, cancellationToken);
    }

    public Task<Message> SendTextMessageAsync(long chatId, string text, CancellationToken cancellationToken)
        => SendTextMessageAsync(
            chatId,
            text,
            null,
            cancellationToken
            );

    public Task SendChatActionAsync(long chatId, ChatAction chatAction, CancellationToken cancellationToken)
    {
        var statistic = new StatisticMessage
        {
            ChatId = chatId,
            ChatAction = chatAction,
            OperationType = nameof(SendChatActionAsync),
        };

        var task = botClient.SendChatActionAsync(
            chatId,
            chatAction,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, statistic, cancellationToken);
    }

    public Task<Message> SendDocumentAsync(long chatId, InputFile document, IReplyMarkup replyMarkup, CancellationToken cancellationToken)
    {
        var statistic = new StatisticMessage
        {
            ChatId = chatId,
            ReplyMarkup = replyMarkup,
            InputFile = document,
            OperationType = nameof(SendDocumentAsync),
        };

        var task = botClient.SendDocumentAsync(
            chatId,
            document,
            parseMode: ParseMode,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, statistic, cancellationToken);
    }

    public Task<Message> SendDocumentAsync(long chatId, InputFile document, CancellationToken cancellationToken)
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
        var statistic = new StatisticMessage
        {
            ChatId = chatId,
            ReplyMarkup = replyMarkup,
            Caption = caption,
            InputFile = photo,
            OperationType = nameof(SendPhotoAsync),
        };

        var task = botClient.SendPhotoAsync(
            chatId,
            photo,
            caption: caption,
            parseMode: ParseMode,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, statistic, cancellationToken);
    }

    public Task<Message> SendPhotoAsync(long chatId, InputFile photo, CancellationToken cancellationToken)
        => SendPhotoAsync(
            chatId,
            photo,
            null,
            null,
            cancellationToken
            );

    public Task DownloadFileAsync(string filePath, Stream destination, CancellationToken cancellationToken)
    {
        var task = botClient.DownloadFileAsync(
            filePath,
            destination,
            cancellationToken
            );

        return ExecuteEnqueueSafety(task, cancellationToken);
    }

    public Task<File> GetFileAsync(string fileId, CancellationToken cancellationToken)
    {
        var task = botClient.GetFileAsync(
            fileId,
            cancellationToken
            );

        return ExecuteEnqueueSafety(task, cancellationToken);
    }
}