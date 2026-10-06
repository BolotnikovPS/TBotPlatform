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
    /// Downloads an image
    /// </summary>
    /// <param name="stateContext">Context used to process the message</param>
    /// <param name="message">The message</param>
    /// <param name="cancellationToken"></param>
    public static Task<IResult<FileData?>> DownloadImage(this IStateContext stateContext, Message? message, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadImage(message, cancellationToken);

    /// <summary>
    /// Downloads a document
    /// </summary>
    /// <param name="stateContext">Context used to process the message</param>
    /// <param name="message">The message</param>
    /// <param name="cancellationToken"></param>
    public static Task<IResult<FileData?>> DownloadDocument(this IStateContext stateContext, Message? message, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadDocument(message, cancellationToken);

    /// <summary>
    /// Downloads a file
    /// </summary>
    /// <param name="stateContext">Context used to process the message</param>
    /// <param name="fileId">File id</param>
    /// <param name="cancellationToken"></param>
    public static Task<IResult<FileData?>> DownloadFile(this IStateContext stateContext, string fileId, CancellationToken cancellationToken)
        => stateContext.TelegramContext.DownloadFileData(fileId, cancellationToken);

    /// <summary>
    /// Downloads an image
    /// </summary>
    /// <param name="telegramContext">Telegram context</param>
    /// <param name="message">The message</param>
    /// <param name="cancellationToken"></param>
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
            || message.Document!.MimeType?.Contains("image") != true
           )
        {
            return FailureResult();
        }

        var photoDocument = message.Document;
        return telegramContext.DownloadFileData(photoDocument.FileId, cancellationToken);
    }

    /// <summary>
    /// Downloads a document
    /// </summary>
    /// <param name="telegramContext">Telegram context</param>
    /// <param name="message">The message</param>
    /// <param name="cancellationToken"></param>
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