using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Extension;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Handlers;

internal partial class TelegramChatHandler
{
    private async Task<FileData> DownloadMessagePhotoAsync(long chatId, Message message, CancellationToken cancellationToken)
    {
        if (message.IsNotNull()
            && message.Photo.CheckAny()
           )
        {
            var photo = message.Photo![^1];
            return await DownloadFileAsync(chatId, photo.FileId, cancellationToken);
        }

        if (message.IsNull()
            || message.Document.IsNull()
            || !message.Document!.MimeType!.Contains("image")
           )
        {
            return default;
        }

        var photoDocument = message.Document;
        return await DownloadFileAsync(chatId, photoDocument.FileId, cancellationToken);
    }

    private async Task<FileData> DownloadMessageDocumentAsync(long chatId, Message message, CancellationToken cancellationToken)
    {
        if (message.IsNull()
            || message.Document.IsNull()
           )
        {
            return default;
        }

        var document = message.Document!;
        return await DownloadFileAsync(chatId, document.FileId, cancellationToken);
    }

    private async Task<FileData> DownloadFileAsync(long chatId, string fileId, CancellationToken cancellationToken)
    {
        if (chatId.IsDefault()
            || fileId.IsNull()
           )
        {
            return default;
        }

        try
        {
            var file = await botClient.GetFileAsync(chatId, fileId, cancellationToken);

            if (file.IsNull())
            {
                return default;
            }

            await using var fileStream = new MemoryStream();

            await botClient.DownloadFileAsync(chatId, file.FilePath!, fileStream, cancellationToken);

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