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
    /// <summary>
    /// Скачивает изображение
    /// </summary>
    /// <param name="stateContext">Контекст для обработки сообщения</param>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<FileData?> DownloadImage(this IStateContext stateContext, Message? message, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadImage(message, cancellationToken);

    /// <summary>
    /// Скачивает документ
    /// </summary>
    /// <param name="stateContext">Контекст для обработки сообщения</param>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<FileData?> DownloadDocument(this IStateContext stateContext, Message? message, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadDocument(message, cancellationToken);

    /// <summary>
    /// Скачивает файл
    /// </summary>
    /// <param name="stateContext">Контекст для обработки сообщения</param>
    /// <param name="fileId">Id файла</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<FileData?> DownloadFile(this IStateContext stateContext, string fileId, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadFile(fileId, cancellationToken);

    /// <summary>
    /// Скачивает изображение
    /// </summary>
    /// <param name="telegramContext">Контекст telegram</param>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<FileData?> DownloadImage(this ITelegramContext telegramContext, Message? message, CancellationToken cancellationToken)
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

    /// <summary>
    /// Скачивает документ 
    /// </summary>
    /// <param name="telegramContext">Контекст telegram</param>
    /// <param name="message">Сообщение </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Скачивает файл
    /// </summary>
    /// <param name="telegramContext">Контекст telegram</param>
    /// <param name="fileId">Id файла</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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