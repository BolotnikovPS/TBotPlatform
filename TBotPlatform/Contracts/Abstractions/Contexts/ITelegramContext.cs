#nullable enable

using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface ITelegramContext : ITelegramBotClient
{
    /// <summary>
    /// Gets the OperationGuid of the current Telegram request pools
    /// </summary>
    Guid CurrentOperation { get; }

    /// <summary>
    /// Gets the bot context settings
    /// </summary>
    TBotSetting GetBotSetting();

    /// <summary>
    /// Downloads a file
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="fileId">File id</param>
    Task<IResult<FileData?>> DownloadFileData(string fileId, CancellationToken cancellationToken);
}