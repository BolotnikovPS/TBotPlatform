using ComposableAsync;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Handlers;

internal class TelegramHttpHandler(ILogger<TelegramHttpHandler> logger, IDispatcher dispatcher)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.TryGetValues(DefaultHeadersConstant.ContextOperation, out var value);
        Guid.TryParse(value?.FirstOrDefault(), out var operationGuid);
        request.Headers.Remove(DefaultHeadersConstant.ContextOperation);

        Exception exception = null;
        var sbLog = new StringBuilder($"Request: {request.ToJson()}");
        try
        {
            if (request.Content.IsNotNull())
            {
                var resultRequest = await request.Content!.ReadAsStringAsync(cancellationToken);
                sbLog.AppendLine(resultRequest.ToJson());
            }

            var response = await dispatcher.Enqueue(() => base.Send(request, cancellationToken), cancellationToken);

            sbLog.AppendLine($"Response: {response.ToJson()}");

            if (response.StatusCode == HttpStatusCode.TooManyRequests
                && response.Headers.RetryAfter.IsNotNull()
               )
            {
                sbLog.AppendLine($"Delay: {response!.Headers!.RetryAfter!.Delta.ToString()}");
            }

            if (response.Content.IsNull())
            {
                return response;
            }

            var resultResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            sbLog.AppendLine($"Response Content: {resultResponse.ToJson()}");

            return response;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            var logLevel = exception.IsNotNull()
                ? LogLevel.Error
                : LogLevel.Debug;

            logger.Log(logLevel, exception, "{operationGuid} {sbLog}", operationGuid, sbLog);
        }
    }
}