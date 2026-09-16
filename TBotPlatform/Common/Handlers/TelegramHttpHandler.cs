#nullable enable
using Microsoft.Extensions.Logging;
using System.Net;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;
using ComposableAsync;

namespace TBotPlatform.Common.Handlers;

internal class TelegramHttpHandler(ILogger<TelegramHttpHandler> logger, IDispatcher dispatcher, TBotSetting botSetting) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var operationGuid = Guid.Empty;
        if (request.Headers.TryGetValues(DefaultHeadersConstant.ContextOperation, out var contextOperationValue) && Guid.TryParse(contextOperationValue.FirstOrDefault(), out operationGuid))
        {
            request.Headers.Remove(DefaultHeadersConstant.ContextOperation);
        }

        Exception? exception = null;
        HttpStatusCode? statusCode = null;
        TimeSpan? retryAfter = null;
        try
        {
            var response = await dispatcher.Enqueue(() => base.SendAsync(request, cancellationToken), cancellationToken);
            statusCode = response.StatusCode;

            if (response.StatusCode == HttpStatusCode.TooManyRequests && response.Headers.RetryAfter.IsNotNull())
            {
                retryAfter = response.Headers.RetryAfter!.Delta;
            }

            if (botSetting.VerboseLog && request.Content.IsNotNull())
            {
                logger.LogDebug("{operationGuid} Request payload: {payload}", operationGuid, await request.Content!.ReadAsStringAsync(cancellationToken));
            }

            return response;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            var logLevel = exception.IsNotNull() ? LogLevel.Error : LogLevel.Debug;
            logger.Log(
                logLevel,
                exception,
                "{operationGuid} {method} {uri} {statusCode} {retryAfter}",
                operationGuid,
                request.Method,
                request.RequestUri,
                statusCode,
                retryAfter);
        }
    }
}
