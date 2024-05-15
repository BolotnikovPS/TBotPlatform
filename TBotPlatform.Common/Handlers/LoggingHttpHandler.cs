using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Handlers;

public class LoggingHttpHandler(ILogger<LoggingHttpHandler> logger)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Exception exception = null;
        var sbLog = new StringBuilder($"Request: {request.ToJson()}");
        try
        {
            if (request.Content.IsNotNull())
            {
                var resultRequest = await request.Content!.ReadAsStringAsync(cancellationToken);
                sbLog.AppendLine(resultRequest.ToJson());
            }

            var response = await base.SendAsync(request, cancellationToken);

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

            logger.Log(logLevel, exception, sbLog.ToString());
        }
    }
}