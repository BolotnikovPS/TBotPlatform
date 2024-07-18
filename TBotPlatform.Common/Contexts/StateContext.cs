using Microsoft.Extensions.Logging;
using System.Text;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.StateContext;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext<T>(
    ILogger logger,
    ITelegramContext botClient
    ) : IStateContext<T>
    where T : UserBase
{
    public MarkupNextState MarkupNextState { get; private set; }
    public T UserDb { get; private set; }
    public ChatMessage ChatMessage { get; private set; }
    public EBindStateType BindState { get; private set; }
    public bool IsForceReplyLastMenu { get; private set; }
    
    private const string ChooseAction = "😊 Выберите действие";
    private const int TextLength = 4096;

    public async Task CreateStateContextAsync(
        T user,
        Update update,
        MarkupNextState markupNextState,
        CancellationToken cancellationToken
        )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ChatMessage = new(
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

    public void SetBindState(EBindStateType type)
    {
        BindState = type;
    }

    public async Task<Message> SendDocumentAsync(
        InputFile inputFile,
        CancellationToken cancellationToken
        )
    {
        if (UserDb.ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        return await botClient.SendDocumentAsync(
            UserDb.ChatId,
            inputFile,
            cancellationToken: cancellationToken
            );
    }

    public async Task SendChatActionAsync(
        ChatAction chatAction,
        CancellationToken cancellationToken
        )
    {
        if (UserDb.ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        await botClient.SendChatActionAsync(
            UserDb.ChatId,
            chatAction,
            cancellationToken: cancellationToken
            );
    }

    public async Task<Message> UpdateMarkupAsync(
        ButtonsRuleMassiveList replyMarkup,
        CancellationToken cancellationToken
        )
    {
        if (UserDb.ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        var newMarkup = Map(replyMarkup);

        if (newMarkup.IsNull())
        {
            throw new ReplyKeyboardMarkupArgException();
        }

        return await botClient.SendTextMessageAsync(
            UserDb.ChatId,
            ChooseAction,
            replyMarkup: newMarkup,
            cancellationToken: cancellationToken
            );
    }

    public async Task UpdateMarkupTextAndDropButtonAsync(
        string text,
        CancellationToken cancellationToken
        )
    {
        if (MarkupNextState.State.IsNull())
        {
            return;
        }

        if (UserDb.ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (!ChatMessage.CallbackQueryMessageIdOrNull.HasValue)
        {
            throw new CallbackQueryMessageIdOrNullArgException();
        }

        if (text.Length > TextLength)
        {
            throw new TextLengthException(text.Length, TextLength);
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
                cancellationToken: cancellationToken
                );

            return;
        }

        await botClient.EditMessageTextAsync(
            UserDb.ChatId,
            ChatMessage.CallbackQueryMessageIdOrNull.Value,
            message,
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
        if (UserDb.ChatId.IsDefault())
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
        if (message.IsNotNull()
            && message.Photo.CheckAny()
           )
        {
            var photo = message.Photo![^1];
            return await DownloadFileAsync(photo.FileId, cancellationToken);
        }

        if (message.IsNull()
            || message.Document.IsNull()
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
        if (fileId.IsNull())
        {
            return default;
        }

        try
        {
            var file = await botClient.GetFileAsync(fileId, cancellationToken);

            if (file.IsNull())
            {
                return default;
            }

            await using var fileStream = new MemoryStream();

            await botClient.DownloadFileAsync(file.FilePath!, fileStream, cancellationToken);

            return new()
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