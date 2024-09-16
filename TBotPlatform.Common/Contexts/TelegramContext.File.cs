using TBotPlatform.Common.Contracts;
using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    public Task<Message> SendDocumentAsync(long chatId, InputFile document, IReplyMarkup replyMarkup, bool disableNotification, CancellationToken cancellationToken)
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            ReplyMarkup = replyMarkup,
            InputFile = document,
            DisableNotification = disableNotification,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(SendDocumentAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };

        var task = _botClient.SendDocumentAsync(
            chatId,
            document,
            parseMode: ParseMode,
            disableNotification: disableNotification,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> SendDocumentAsync(long chatId, InputFile document, bool disableNotification, CancellationToken cancellationToken)
        => SendDocumentAsync(chatId, document, replyMarkup: null, disableNotification, cancellationToken);

    public Task<Message> SendPhotoAsync(
        long chatId,
        InputFile photo,
        string caption,
        IReplyMarkup replyMarkup,
        bool disableNotification,
        CancellationToken cancellationToken
        )
    {
        var logMessageData = new TelegramContextLogMessageData
        {
            ReplyMarkup = replyMarkup,
            Caption = caption,
            InputFile = photo,
            DisableNotification = disableNotification,
        };

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(SendPhotoAsync),
            ChatId = chatId,
            MessageBody = logMessageData,
        };

        var task = _botClient.SendPhotoAsync(
            chatId,
            photo,
            caption: caption,
            parseMode: ParseMode,
            disableNotification: disableNotification,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> SendPhotoAsync(long chatId, InputFile photo, bool disableNotification, CancellationToken cancellationToken)
        => SendPhotoAsync(chatId, photo, caption: null, replyMarkup: null, disableNotification, cancellationToken);

    public Task DownloadFileAsync(long chatId, string filePath, Stream destination, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(DownloadFileAsync),
            ChatId = chatId,
            MessageBody = "",
        };

        var task = _botClient.DownloadFileAsync(filePath, destination, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<File> GetFileAsync(long chatId, string fileId, CancellationToken cancellationToken)
    {
        var task = _botClient.GetFileAsync(fileId, cancellationToken);

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(GetFileAsync),
            ChatId = chatId,
            MessageBody = "",
        };

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<File> GetInfoAndDownloadFileAsync(long chatId, string fileId, Stream destination, CancellationToken cancellationToken)
    {
        var task = _botClient.GetInfoAndDownloadFileAsync(fileId, destination, cancellationToken);

        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(GetFileAsync),
            ChatId = chatId,
            MessageBody = "",
        };

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}