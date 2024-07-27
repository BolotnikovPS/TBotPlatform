using Microsoft.Extensions.Logging;
using System.Text;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.StateContext;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext(ILogger logger, ITelegramContext botClient) : IStateContext
{
    public MarkupNextState MarkupNextState { get; private set; }
    public ChatMessage ChatMessage { get; private set; }
    public EBindStateType BindState { get; private set; }
    public bool IsForceReplyLastMenu { get; private set; }

    private long ChatId { get; set; }

    private const string ChooseAction = "😊 Выберите действие";
    private const int TextLength = 4096;

    public async Task CreateStateContextAsync(
        UserBase user,
        Update update,
        MarkupNextState markupNextState,
        CancellationToken cancellationToken
        )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user.ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        ChatId = user.ChatId;

        ChatMessage = new(
            update?.Message?.Text,
            update?.Message?.ReplyToMessage?.Text,
            update?.CallbackQuery,
            await DownloadMessagePhotoAsync(update?.Message, cancellationToken)
            );

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

    public Task<Message> SendDocumentAsync(InputFile inputFile, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        return botClient.SendDocumentAsync(
            ChatId,
            inputFile,
            cancellationToken
            );
    }

    public Task SendChatActionAsync(ChatAction chatAction, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        return botClient.SendChatActionAsync(
            ChatId,
            chatAction,
            cancellationToken
            );
    }

    public Task<Message> UpdateMarkupAsync(ButtonsRuleMassiveList replyMarkup, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        var newMarkup = Map(replyMarkup);

        if (newMarkup.IsNull())
        {
            throw new ReplyKeyboardMarkupArgException();
        }

        return botClient.SendTextMessageAsync(
            ChatId,
            ChooseAction,
            newMarkup,
            cancellationToken
            );
    }

    public Task UpdateMarkupTextAndDropButtonAsync(string text, CancellationToken cancellationToken)
    {
        if (MarkupNextState.State.IsNull())
        {
            return Task.CompletedTask;
        }

        if (ChatId.IsDefault())
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
            return botClient.EditMessageCaptionAsync(
                ChatId,
                ChatMessage.CallbackQueryMessageIdOrNull.Value,
                message,
                cancellationToken
                );
        }

        return botClient.EditMessageTextAsync(
            ChatId,
            ChatMessage.CallbackQueryMessageIdOrNull.Value,
            message,
            cancellationToken
            );
    }

    public Task UpdateMarkupTextAndDropButtonAsync(CancellationToken cancellationToken)
        => UpdateMarkupTextAndDropButtonAsync(
            "",
            cancellationToken
            );

    public Task RemoveCurrentReplyMessageAsync(CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (!ChatMessage.CallbackQueryMessageIdOrNull.HasValue)
        {
            throw new CallbackQueryMessageIdOrNullArgException();
        }

        return botClient.DeleteMessageAsync(
            ChatId,
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
        ChatId = 0;
        MarkupNextState = null;
        IsForceReplyLastMenu = false;
    }

    private async Task<FileData> DownloadMessagePhotoAsync(Message message, CancellationToken cancellationToken)
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
            || !message.Document!.MimeType!.Contains("image")
           )
        {
            return default;
        }

        var photoDocument = message.Document;
        return await DownloadFileAsync(photoDocument.FileId, cancellationToken);
    }

    private async Task<FileData> DownloadFileAsync(string fileId, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (fileId.IsNull())
        {
            return default;
        }

        try
        {
            var file = await botClient.GetFileAsync(ChatId, fileId, cancellationToken);

            if (file.IsNull())
            {
                return default;
            }

            await using var fileStream = new MemoryStream();

            await botClient.DownloadFileAsync(ChatId, file.FilePath!, fileStream, cancellationToken);

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