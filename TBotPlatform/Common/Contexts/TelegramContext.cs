#nullable enable

using Microsoft.IO;
using System.Diagnostics;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Extension;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Contexts;

internal class TelegramContext : TelegramBotClient, ITelegramContext, IAsyncDisposable
{
    private readonly ITelegramContextLog _telegramContextLog;
    private readonly TBotSetting _botSetting;
    private readonly RecyclableMemoryStreamManager _mgr;
    private readonly Stopwatch _timer = new();
    private int _iteration;

    public TelegramContext(HttpClient client, TBotSetting botSetting, ITelegramContextLog telegramContextLog, RecyclableMemoryStreamManager mgr)
        : base(botSetting.Token ?? throw new ArgumentException("Token"), client)
    {
        client.DefaultRequestHeaders.TryAddWithoutValidation(DefaultHeadersConstant.ContextOperation, CurrentOperation.ToString());

        _botSetting = botSetting;
        _telegramContextLog = telegramContextLog;
        _mgr = mgr;
    }

    public Guid CurrentOperation { get; } = Guid.NewGuid();

    public override async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var properties = request.GetType().GetProperties();

        var chatIdValue = properties.FirstOrDefault(z => z.Name == "ChatId")?.GetValue(request) as ChatId;
        var methodValue = properties.FirstOrDefault(z => z.Name == "MethodName")?.GetValue(request) as string;

        var protectContentValue = properties.FirstOrDefault(z => z.Name == "ProtectContent");
        if (protectContentValue.IsNotNull() && _botSetting.ProtectContent)
        {
            protectContentValue?.SetValue(request, _botSetting.ProtectContent);
        }

        var fullLogMessage = new TelegramContextFullLogMessage
        {
            Request = new()
            {
                ChatId = chatIdValue?.Identifier ?? 0,
                OperationGuid = CurrentOperation,
                OperationType = methodValue ?? "",
                MessageBody = properties
                             .Where(z => z.Name.NotIn("HttpMethod", "MethodName", "IsWebhookResponse", "ChatId"))
                             .ToDictionary(
                                  property => property.Name,
                                  property => property.PropertyType != typeof(InputFile) ? property.GetValue(request)?.ToString() : string.Empty
                                  ),
            },
        };

        try
        {
            _timer.Start();

            var result = await base.SendRequest(request, cancellationToken);

            if (result.IsNotNull())
            {
                fullLogMessage.Result = result;
            }

            await _telegramContextLog.HandleLog(fullLogMessage, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _iteration++;
            _timer.Stop();

            await _telegramContextLog.HandleErrorLog(fullLogMessage, ex, cancellationToken);
            throw;
        }
    }

    public TBotSetting GetBotSetting() => _botSetting;

    public async Task<IResult<FileData?>> DownloadFileData(string fileId, CancellationToken cancellationToken)
    {
        var file = await this.GetFile(fileId, cancellationToken);

        if (file.IsNull())
        {
            return ResultT<FileData?>.Failure(ErrorResult.NotFound(""));
        }

        await using var fileStream = _mgr.GetStream();

        await DownloadFile(file.FilePath!, fileStream, cancellationToken);

        return ResultT<FileData>.Success(new()
        {
            Bytes = fileStream.GetBuffer(),
            Name = file.FilePath,
            Size = file.FileSize!.Value,
            FileId = fileId,
        });
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _telegramContextLog.HandleEnqueueLog(_iteration, _timer.Elapsed.Milliseconds, CurrentOperation, CancellationToken.None);
        }
        catch
        {
            // ignored
        }
    }
}