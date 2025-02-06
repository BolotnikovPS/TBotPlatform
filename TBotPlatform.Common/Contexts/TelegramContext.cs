using System.Diagnostics;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Contexts;

internal class TelegramContext : TelegramBotClient, ITelegramContext, IAsyncDisposable
{
    private readonly ITelegramContextLog _telegramContextLog;
    private readonly TelegramSettings _telegramSettings;
    private readonly Guid _operationGuid = Guid.NewGuid();

    private readonly Stopwatch _timer = new();
    private int _iteration;

    public TelegramContext(HttpClient client, TelegramSettings telegramSettings, ITelegramContextLog telegramContextLog)
        : base(telegramSettings.Token ?? throw new ArgumentException("Token"), client)
    {
        client.DefaultRequestHeaders.TryAddWithoutValidation(DefaultHeadersConstant.ContextOperation, _operationGuid.ToString());

        _telegramSettings = telegramSettings;
        _telegramContextLog = telegramContextLog;
    }

    public Guid CurrentOperation => _operationGuid;

    public override async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var properties = request.GetType().GetProperties();

        var chatIdValue = properties.FirstOrDefault(z => z.Name == "ChatId")?.GetValue(request) as ChatId;
        var methodValue = properties.FirstOrDefault(z => z.Name == "MethodName")?.GetValue(request) as string;

        var protectContentValue = properties.FirstOrDefault(z => z.Name == "ProtectContent");
        if (protectContentValue.IsNotNull())
        {
            protectContentValue?.SetValue(request, _telegramSettings.ProtectContent);
        }

        var fullLogMessage = new TelegramContextFullLogMessage
        {
            Request = new()
            {
                ChatId = chatIdValue?.Identifier ?? 0,
                OperationGuid = _operationGuid,
                OperationType = methodValue ?? "",
                MessageBody = properties
                             .Where(z => z.Name.NotIn("HttpMethod", "MethodName", "IsWebhookResponse", "ChatId"))
                             .ToDictionary(property => property.Name, property => property.GetValue(request)?.ToString()),
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

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _telegramContextLog.HandleEnqueueLog(_iteration, _timer.Elapsed.Milliseconds, _operationGuid, CancellationToken.None);
        }
        catch
        {
            // ignored
        }
    }
}