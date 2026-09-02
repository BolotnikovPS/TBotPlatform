#nullable enable

using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface ITelegramContext : ITelegramBotClient
{
    /// <summary>
    /// Получает OperationGuid текущих пулов запросов к telegram
    /// </summary>
    /// <returns></returns>
    Guid CurrentOperation { get; }

    /// <summary>
    /// Получение настроек контекста бота
    /// </summary>
    /// <returns></returns>
    TBotSetting GetBotSetting();

    /// <summary>
    /// Скачивает файл
    /// </summary>
    /// <param name="fileId">Id файла</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult<FileData?>> DownloadFileData(string fileId, CancellationToken cancellationToken);
}