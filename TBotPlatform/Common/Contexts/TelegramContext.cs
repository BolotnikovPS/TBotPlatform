#nullable enable

using Microsoft.IO;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
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
    private static readonly string[] ExcludedLogProperties = ["HttpMethod", "MethodName", "IsWebhookResponse", "ChatId"];

    private readonly ITelegramContextLog _telegramContextLog;
    private readonly TBotSetting _botSetting;
    private readonly RecyclableMemoryStreamManager _mgr;
    private int _requestCount;
    private long _elapsedMilliseconds;

    private static readonly ConcurrentDictionary<Type, RequestMeta> RequestMetaCache = new();

    private static RequestMeta GetRequestMeta(Type type) => RequestMetaCache.GetOrAdd(type, static t => RequestMeta.Create(t));

    public TelegramContext(HttpClient client, TBotSetting botSetting, ITelegramContextLog telegramContextLog, RecyclableMemoryStreamManager mgr)
        : base(CreateClientOptions(botSetting), client)
    {
        if (botSetting.RequestTimeout is { } requestTimeout)
        {
            Timeout = requestTimeout;
        }

        client.DefaultRequestHeaders.TryAddWithoutValidation(DefaultHeadersConstant.ContextOperation, CurrentOperation.ToString());

        _botSetting = botSetting;
        _telegramContextLog = telegramContextLog;
        _mgr = mgr;
    }

    private static TelegramBotClientOptions CreateClientOptions(TBotSetting botSetting)
    {
        if (botSetting.Token.IsNull())
        {
            throw new ArgumentException("Token", nameof(botSetting));
        }

        var options = new TelegramBotClientOptions(botSetting.Token!, botSetting.BaseUrl, botSetting.UseTestEnvironment);

        if (botSetting.RetryCount is { } retryCount)
        {
            options.RetryCount = retryCount;
        }

        if (botSetting.RetryThreshold is { } retryThreshold)
        {
            options.RetryThreshold = retryThreshold;
        }

        return options;
    }

    /// <summary>
    /// Request metadata computed once per type: a fast ProtectContent setter
    /// (a compiled expression instead of reflection on every call) and the properties used for verbose logging.
    /// </summary>
    private sealed class RequestMeta
    {
        private readonly PropertyInfo[] _verboseProperties;

        private RequestMeta(Action<object, bool>? protectContentSetter, PropertyInfo[] verboseProperties)
        {
            ProtectContentSetter = protectContentSetter;
            _verboseProperties = verboseProperties;
        }

        public Action<object, bool>? ProtectContentSetter { get; }

        public Dictionary<string, string?> GetMessageBody(IRequest request)
            => _verboseProperties.ToDictionary(
                property => property.Name,
                property => property.PropertyType != typeof(InputFile) ? property.GetValue(request)?.ToString() : string.Empty
                );

        public static RequestMeta Create(Type type)
        {
            var protectContentProperty = type.GetProperty("ProtectContent");

            var verboseProperties = type
                .GetProperties()
                .Where(z => z.CanRead && z.GetIndexParameters().Length == 0 && z.Name.NotIn(ExcludedLogProperties))
                .ToArray();

            return new RequestMeta(CreateProtectContentSetter(protectContentProperty), verboseProperties);
        }

        private static Action<object, bool>? CreateProtectContentSetter(PropertyInfo? property)
        {
            if (property is null || !property.CanWrite || property.PropertyType != typeof(bool))
            {
                return null;
            }

            var target = Expression.Parameter(typeof(object), "target");
            var value = Expression.Parameter(typeof(bool), "value");

            var body = Expression.Assign(
                Expression.Property(Expression.Convert(target, property.DeclaringType!), property),
                value
                );

            return Expression.Lambda<Action<object, bool>>(body, target, value).Compile();
        }
    }

    public Guid CurrentOperation { get; } = Guid.NewGuid();

    public override async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestMeta = GetRequestMeta(request.GetType());

        var chatIdValue = request is IChatTargetable chatTargetable ? chatTargetable.ChatId : null;
        var methodValue = request.MethodName;

        if (_botSetting.ProtectContent && requestMeta.ProtectContentSetter is not null)
        {
            requestMeta.ProtectContentSetter(request, true);
        }

        Dictionary<string, string?> messageBody = [];
        if (_botSetting.VerboseLog)
        {
            messageBody = requestMeta.GetMessageBody(request);
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

        var startedAt = Stopwatch.GetTimestamp();

        try
        {
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
            await _telegramContextLog.HandleErrorLog(fullLogMessage, ex, cancellationToken);
            throw;
        }
        finally
        {
            Interlocked.Increment(ref _requestCount);
            Interlocked.Add(ref _elapsedMilliseconds, (long)Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);
        }
    }

    public TBotSetting GetBotSetting() => _botSetting;

    public async Task<IResult<FileData?>> DownloadFileData(string fileId, CancellationToken cancellationToken)
    {
        await using var fileStream = _mgr.GetStream();

        try
        {
            // GetInfoAndDownloadFile gets file metadata and writes content to stream immediately.
            var file = await this.GetInfoAndDownloadFile(fileId, fileStream, cancellationToken);

            fileStream.Position = 0;

            var bytes = new byte[fileStream.Length];
            await fileStream.ReadExactlyAsync(bytes, cancellationToken);

            return ResultT<FileData>.Success(new()
            {
                Bytes = bytes,
                Name = file.FilePath!,
                Size = file.FileSize ?? bytes.Length,
                FileId = fileId,
            });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            // The file is unavailable (stale file_id, too large size, etc.) - return Failure,
            // as before, so the calling code works with Result instead of an exception.
            return ResultT<FileData?>.Failure(ErrorResult.NotFound(""));
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            var elapsedMilliseconds = (int)Math.Min(_elapsedMilliseconds, int.MaxValue);

            await _telegramContextLog.HandleEnqueueLog(_requestCount, elapsedMilliseconds, CurrentOperation, CancellationToken.None);
        }
        catch
        {
            // ignored
        }
    }
}