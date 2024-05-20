using Microsoft.Extensions.Logging;
using System.Text;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using PMode = Telegram.Bot.Types.Enums.ParseMode;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext<T>(
    ILogger logger,
    ITelegramBotClient botClient
    ) : IStateContext<T>
    where T : UserBase
{
    public MarkupNextState MarkupNextState { get; private set; }
    public T UserDb { get; private set; }
    public ChatMessage ChatMessage { get; private set; }
    public bool IsForceReplyLastMenu { get; private set; }

    private bool ProtectContent { get; set; }

    private const string ChooseAction = "😊 Выберите действие";
    private const PMode ParseMode = PMode.Html;
    private const int TextLength = 4096;

    public async Task CreateStateContextAsync(
        T user,
        Update update,
        MarkupNextState markupNextState,
        CancellationToken cancellationToken,
        bool protectContent = false
        )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ProtectContent = protectContent;

        ChatMessage = new ChatMessage(
            update?.Message?.Text,
            update?.Message?.ReplyToMessage?.Text,
            update?.CallbackQuery,
            await DownloadMessagePhotoAsync(update?.Message, cancellationToken)
            );

        UserDb = user;
        MarkupNextState = markupNextState;
    }

    public void SetNeedIsForceReplyLastMenu()
    {
        IsForceReplyLastMenu = true;
    }

    public Task<Message> SendDocumentAsync(
        InputFile inputFile,
        CancellationToken cancellationToken
        )
    {
        if (!UserDb.ChatId.CheckAny())
        {
            throw new ChatIdArgException();
        }

        return botClient.SendDocumentAsync(
            UserDb.ChatId,
            inputFile,
            parseMode: ParseMode,
            protectContent: ProtectContent,
            cancellationToken: cancellationToken
            );
    }

    public async Task SendChatActionAsync(
        ChatAction chatAction,
        CancellationToken cancellationToken
        )
    {
        if (!UserDb.ChatId.CheckAny())
        {
            throw new ChatIdArgException();
        }

        await botClient.SendChatActionAsync(
            UserDb.ChatId,
            chatAction,
            cancellationToken: cancellationToken
            );
    }

    public Task<Message> UpdateMarkupAsync(
        ButtonsRuleMassivList replyMarkup,
        CancellationToken cancellationToken
        )
    {
        if (!UserDb.ChatId.CheckAny())
        {
            throw new ChatIdArgException();
        }

        var newMarkup = Map(replyMarkup);

        if (!newMarkup.CheckAny())
        {
            throw new ReplyKeyboardMarkupArgException();
        }

        return botClient.SendTextMessageAsync(
            UserDb.ChatId,
            ChooseAction,
            parseMode: ParseMode,
            protectContent: ProtectContent,
            replyMarkup: newMarkup,
            cancellationToken: cancellationToken
            );
    }

    public async Task UpdateMarkupTextAndDropButtonAsync(
        string text,
        CancellationToken cancellationToken
        )
    {
        if (!MarkupNextState.State.CheckAny())
        {
            return;
        }

        if (!UserDb.ChatId.CheckAny())
        {
            throw new ChatIdArgException();
        }

        if (!ChatMessage.CallbackQueryMessageIdOrNull.HasValue)
        {
            throw new CallbackQueryMessageIdOrNullArgException();
        }

        if (text.Length > TextLength)
        {
            throw new TextLengthException();
        }

        var message = new StringBuilder(ChatMessage?.CallbackQueryMessageOrNull)
                     .AppendLine()
                     .AppendLine(text)
                     .ToString();

        if (ChatMessage!.CallbackQueryMessageWithCaption)
        {
            await botClient.EditMessageCaptionAsync(
                UserDb.ChatId,
                ChatMessage.CallbackQueryMessageIdOrNull.Value,
                message,
                ParseMode,
                cancellationToken: cancellationToken
                );

            return;
        }

        await botClient.EditMessageTextAsync(
            UserDb.ChatId,
            ChatMessage.CallbackQueryMessageIdOrNull.Value,
            message,
            ParseMode,
            cancellationToken: cancellationToken
            );
    }

    public async Task UpdateMarkupTextAndDropButtonAsync(
        CancellationToken cancellationToken
        )
    {
        await UpdateMarkupTextAndDropButtonAsync(
            "",
            cancellationToken
            );
    }

    public async Task RemoveCurrentReplyMessageAsync(CancellationToken cancellationToken)
    {
        if (!UserDb.ChatId.CheckAny())
        {
            throw new ChatIdArgException();
        }

        if (!ChatMessage.CallbackQueryMessageIdOrNull.HasValue)
        {
            throw new CallbackQueryMessageIdOrNullArgException();
        }

        await botClient.DeleteMessageAsync(
            UserDb.ChatId,
            ChatMessage.CallbackQueryMessageIdOrNull.Value,
            cancellationToken
            );
    }

    public ValueTask DisposeAsync()
    {
        CleanUp();

        return ValueTask.CompletedTask;
    }

    private void CleanUp()
    {
        ChatMessage = null;
        UserDb = null;
        MarkupNextState = null;
        IsForceReplyLastMenu = false;
    }

    private async Task<FileData> DownloadMessagePhotoAsync(
        Message message,
        CancellationToken cancellationToken
        )
    {
        if (message.CheckAny()
            && message.Photo.CheckAny()
           )
        {
            var photo = message.Photo![^1];
            return await DownloadFileAsync(photo.FileId, cancellationToken);
        }

        if (!message.CheckAny()
            || !message.Document.CheckAny()
            || !message.Document!.MimeType!.Contains("image"))
        {
            return default;
        }

        var photoDocument = message.Document;
        return await DownloadFileAsync(photoDocument.FileId, cancellationToken);
    }

    private async Task<FileData> DownloadFileAsync(
        string fileId,
        CancellationToken cancellationToken
        )
    {
        if (!fileId.CheckAny())
        {
            return default;
        }

        try
        {
            var file = await botClient.GetFileAsync(fileId, cancellationToken);

            if (!file.CheckAny())
            {
                return default;
            }

            await using var fileStream = new MemoryStream();

            await botClient.DownloadFileAsync(file.FilePath!, fileStream, cancellationToken);

            return new FileData
            {
                Byte = fileStream.ToArray(),
                Name = file.FilePath,
                Size = file.FileSize!.Value,
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка скачивания {fileId}", fileId);
        }

        return default;
    }
}