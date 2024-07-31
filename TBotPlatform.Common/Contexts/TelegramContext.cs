using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;
using PMode = Telegram.Bot.Types.Enums.ParseMode;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext(ILogger<TelegramContext> logger, HttpClient client, TelegramSettings telegramSettings, ITelegramContextLog telegramContextLog) : ITelegramContext
{
    private const PMode ParseMode = PMode.Html;

    private readonly TelegramBotClient _botClient = new(telegramSettings.Token, client);
    private readonly Guid _operationGuid = Guid.NewGuid();

    public Guid GetCurrentOperation() => _operationGuid;

    public Task<Update[]> GetUpdatesAsync(int offset, UpdateType[] allowedUpdates, CancellationToken cancellationToken)
    {
        var task = _botClient.GetUpdatesAsync(
            offset,
            allowedUpdates: allowedUpdates.IsNotNull() ? allowedUpdates : null,
            cancellationToken: cancellationToken
            );

        return Enqueue(() => task, cancellationToken);
    }

    public Task<User> GetBotInfoAsync(CancellationToken cancellationToken)
    {
        return Enqueue(
            () => _botClient.GetMeAsync(cancellationToken),
            cancellationToken
            );
    }

    public Task DeleteMessageAsync(long chatId, int messageId, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(DeleteMessageAsync),
            ChatId = chatId,
            MessageId = messageId,
        };

        var task = _botClient.DeleteMessageAsync(chatId, messageId, cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> EditMessageTextAsync(
        long chatId,
        int messageId,
        string text,
        InlineKeyboardMarkup replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(EditMessageTextAsync),
            ChatId = chatId,
            MessageId = messageId,
            Message = text,
            InlineKeyboardMarkup = replyMarkup,
        };

        var task = _botClient.EditMessageTextAsync(
            chatId,
            messageId,
            text,
            ParseMode,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> EditMessageTextAsync(long chatId, int messageId, string text, CancellationToken cancellationToken)
        => EditMessageTextAsync(chatId, messageId, text, null, cancellationToken);

    public Task<Message> EditMessageCaptionAsync(long chatId, int messageId, string caption, InlineKeyboardMarkup replyMarkup, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(EditMessageCaptionAsync),
            ChatId = chatId,
            MessageId = messageId,
            InlineKeyboardMarkup = replyMarkup,
            Caption = caption,
        };

        var task = _botClient.EditMessageCaptionAsync(
            chatId,
            messageId,
            caption,
            ParseMode,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> EditMessageCaptionAsync(long chatId, int messageId, string caption, CancellationToken cancellationToken)
        => EditMessageCaptionAsync(chatId, messageId, caption, null, cancellationToken);

    public Task<Message> SendTextMessageAsync(long chatId, string text, IReplyMarkup replyMarkup, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(SendTextMessageAsync),
            ChatId = chatId,
            Message = text,
            ReplyMarkup = replyMarkup,
        };

        var task = _botClient.SendTextMessageAsync(
            chatId,
            text,
            parseMode: ParseMode,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> SendTextMessageAsync(long chatId, string text, CancellationToken cancellationToken)
        => SendTextMessageAsync(chatId, text, null, cancellationToken);

    public Task SendChatActionAsync(long chatId, ChatAction chatAction, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(SendChatActionAsync),
            ChatId = chatId,
            ChatAction = chatAction,
        };

        var task = _botClient.SendChatActionAsync(chatId, chatAction, cancellationToken: cancellationToken);

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> SendDocumentAsync(long chatId, InputFile document, IReplyMarkup replyMarkup, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(SendDocumentAsync),
            ChatId = chatId,
            ReplyMarkup = replyMarkup,
            InputFile = document,
        };

        var task = _botClient.SendDocumentAsync(
            chatId,
            document,
            parseMode: ParseMode,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> SendDocumentAsync(long chatId, InputFile document, CancellationToken cancellationToken)
        => SendDocumentAsync(chatId, document, null, cancellationToken);

    public Task<Message> SendPhotoAsync(
        long chatId,
        InputFile photo,
        string caption,
        IReplyMarkup replyMarkup,
        CancellationToken cancellationToken
        )
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(SendPhotoAsync),
            ChatId = chatId,
            ReplyMarkup = replyMarkup,
            Caption = caption,
            InputFile = photo,
        };

        var task = _botClient.SendPhotoAsync(
            chatId,
            photo,
            caption: caption,
            parseMode: ParseMode,
            protectContent: telegramSettings.ProtectContent,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
            );

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }

    public Task<Message> SendPhotoAsync(long chatId, InputFile photo, CancellationToken cancellationToken)
        => SendPhotoAsync(chatId, photo, null, null, cancellationToken);

    public Task DownloadFileAsync(long chatId, string filePath, Stream destination, CancellationToken cancellationToken)
    {
        var log = new TelegramContextLogMessage
        {
            OperationGuid = _operationGuid,
            OperationType = nameof(DownloadFileAsync),
            ChatId = chatId,
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
        };

        return ExecuteEnqueueSafety(task, log, cancellationToken);
    }
}