#nullable enable

using Microsoft.IO;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
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

    private static readonly ConcurrentDictionary<Type, RequestMeta> RequestMetaCache = new();

    private static RequestMeta GetRequestMeta(Type type)
        => RequestMetaCache.GetOrAdd(type, static t => new(t.GetProperty("ChatId"), t.GetProperty("ProtectContent")));

    private sealed record RequestMeta(PropertyInfo? ChatId, PropertyInfo? ProtectContent);

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
        var requestMeta = GetRequestMeta(request.GetType());

        var chatIdValue = requestMeta.ChatId?.GetValue(request) as ChatId;
        var methodValue = request.MethodName;

        var protectContentProperty = requestMeta.ProtectContent;
        if (protectContentProperty is not null && _botSetting.ProtectContent)
        {
            protectContentProperty.SetValue(request, _botSetting.ProtectContent);
        }

        Dictionary<string, string?> messageBody = [];
        if (_botSetting.VerboseLog)
        {
            messageBody = request.GetType()
                                 .GetProperties()
                                 .Where(z => z.Name.NotIn("HttpMethod", "MethodName", "IsWebhookResponse", "ChatId"))
                                 .ToDictionary(
                                      property => property.Name,
                                      property => property.PropertyType != typeof(InputFile) ? property.GetValue(request)?.ToString() : string.Empty
                                      );
        }

        var fullLogMessage = new TelegramContextFullLogMessage
        {
            Request = new()
            {
                ChatId = chatIdValue?.Identifier ?? 0,
                OperationGuid = CurrentOperation,
                OperationType = methodValue ?? "",
                MessageBody = messageBody,
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
        fileStream.Position = 0;
        var bytes = new byte[fileStream.Length];
        _ = await fileStream.ReadAsync(bytes, cancellationToken);

        return ResultT<FileData>.Success(new()
        {
            Bytes = bytes,
            Name = file.FilePath!,
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