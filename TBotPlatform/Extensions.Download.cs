#nullable enable
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Extension;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
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
    public static Task<IResult<FileData?>> DownloadImage(this IStateContext stateContext, Message? message, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadImage(message, cancellationToken);

    /// <summary>
    /// Скачивает документ
    /// </summary>
    /// <param name="stateContext">Контекст для обработки сообщения</param>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IResult<FileData?>> DownloadDocument(this IStateContext stateContext, Message? message, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadDocument(message, cancellationToken);

    /// <summary>
    /// Скачивает файл
    /// </summary>
    /// <param name="stateContext">Контекст для обработки сообщения</param>
    /// <param name="fileId">Id файла</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IResult<FileData?>> DownloadFile(this IStateContext stateContext, string fileId, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadFileData(fileId, cancellationToken);

    /// <summary>
    /// Скачивает изображение
    /// </summary>
    /// <param name="telegramContext">Контекст telegram</param>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IResult<FileData?>> DownloadImage(this ITelegramContext telegramContext, Message? message, CancellationToken cancellationToken)
    {
        if (message.IsNotNull()
            && message!.Photo.CheckAny()
           )
        {
            var photo = message.Photo![^1];
            return telegramContext.DownloadFileData(photo.FileId, cancellationToken);
        }

        if (message.IsNull()
            || message!.Document.IsNull()
            || !message.Document!.MimeType!.Contains("image")
           )
        {
            return FailureResult();
        }

        var photoDocument = message.Document;
        return telegramContext.DownloadFileData(photoDocument.FileId, cancellationToken);
    }

    /// <summary>
    /// Скачивает документ 
    /// </summary>
    /// <param name="telegramContext">Контекст telegram</param>
    /// <param name="message">Сообщение </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<IResult<FileData?>> DownloadDocument(this ITelegramContext telegramContext, Message? message, CancellationToken cancellationToken)
    {
        if (message.IsNull()
            || message!.Document.IsNull()
           )
        {
            return FailureResult();
        }

        var document = message.Document!;
        return telegramContext.DownloadFileData(document.FileId, cancellationToken);
    }

    private static Task<IResult<FileData?>> FailureResult()
    {
        var resp = ResultT<FileData?>.Failure(ErrorResult.NotFound(string.Empty));
        return Task.FromResult<IResult<FileData?>>(resp);
    }
}