#nullable enable
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBotPlatform.Common;

public static partial class Extensions
{
    public static bool ContainPhotoInMessage(this Message? message) => message?.Document?.MimeType?.Contains("image") == true || message?.Photo != null;

    public static bool ContainDocumentInMessage(this Message? message) => !message.ContainPhotoInMessage() && message?.Document?.IsNotNull() == true;

    public static Task<FileData?> DownloadPhotoAsync(this IStateContext stateContext, Message message, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadPhotoAsync(message, cancellationToken);

    public static Task<FileData?> DownloadDocumentAsync(this IStateContext stateContext, Message message, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadDocumentAsync(message, cancellationToken);

    public static Task<FileData?> DownloadFileAsync(this IStateContext stateContext, string fileId, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadFileAsync(fileId, cancellationToken);

    public static Task<FileData?> DownloadPhotoAsync(this ITelegramContext telegramContext, Message message, CancellationToken cancellationToken)
    {
        if (message.IsNotNull()
            && message.Photo.CheckAny()
           )
        {
            var photo = message.Photo![^1];
            return DownloadFileAsync(telegramContext, photo.FileId, cancellationToken);
        }

        if (message.IsNull()
            || message.Document.IsNull()
            || !message.Document!.MimeType!.Contains("image")
           )
        {
            return Task.FromResult<FileData?>(null);
        }

        var photoDocument = message.Document;
        return DownloadFileAsync(telegramContext, photoDocument.FileId, cancellationToken);
    }

    public static Task<FileData?> DownloadDocumentAsync(this ITelegramContext telegramContext, Message message, CancellationToken cancellationToken)
    {
        if (message.IsNull()
            || message.Document.IsNull()
           )
        {
            return Task.FromResult<FileData?>(null);
        }

        var document = message.Document!;
        return DownloadFileAsync(telegramContext, document.FileId, cancellationToken);
    }

    public static async Task<FileData?> DownloadFileAsync(this ITelegramContext telegramContext, string fileId, CancellationToken cancellationToken)
    {
        var file = await telegramContext.GetFile(fileId, cancellationToken);

        if (file.IsNull())
        {
            return default;
        }

        await using var fileStream = new MemoryStream();

        await telegramContext.DownloadFile(file.FilePath!, fileStream, cancellationToken);

        return new()
        {
            Bytes = fileStream.ToArray(),
            Name = file.FilePath,
            Size = file.FileSize!.Value,
            FileId = fileId,
        };
    }
}