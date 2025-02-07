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
    public static Task<FileData?> DownloadPhoto(this IStateContext stateContext, Message? message, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadPhoto(message, cancellationToken);

    public static Task<FileData?> DownloadDocument(this IStateContext stateContext, Message? message, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadDocument(message, cancellationToken);

    public static Task<FileData?> DownloadFile(this IStateContext stateContext, string fileId, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadFile(fileId, cancellationToken);

    public static Task<FileData?> DownloadPhoto(this ITelegramContext telegramContext, Message? message, CancellationToken cancellationToken)
    {
        if (message.IsNotNull()
            && message!.Photo.CheckAny()
           )
        {
            var photo = message.Photo![^1];
            return DownloadFile(telegramContext, photo.FileId, cancellationToken);
        }

        if (message.IsNull()
            || message!.Document.IsNull()
            || !message.Document!.MimeType!.Contains("image")
           )
        {
            return Task.FromResult<FileData?>(null);
        }

        var photoDocument = message.Document;
        return DownloadFile(telegramContext, photoDocument.FileId, cancellationToken);
    }

    public static Task<FileData?> DownloadDocument(this ITelegramContext telegramContext, Message? message, CancellationToken cancellationToken)
    {
        if (message.IsNull()
            || message!.Document.IsNull()
           )
        {
            return Task.FromResult<FileData?>(null);
        }

        var document = message.Document!;
        return DownloadFile(telegramContext, document.FileId, cancellationToken);
    }

    public static async Task<FileData?> DownloadFile(this ITelegramContext telegramContext, string fileId, CancellationToken cancellationToken)
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