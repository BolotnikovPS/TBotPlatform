using ComposableAsync;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Polly;
using Polly.Extensions.Http;
using RateLimiter;
using System.Net;
using TBotPlatform.Common.Contexts;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Builder;

internal partial class BotBuilder
{
    internal void AddBotTelegramContext()
    {
        serviceCollection
           .AddKeyedSingleton(typeof(IDispatcher), botSetting.BotName, (z, s) => GetLimeLimiter(botSetting.HttpPolicy.TelegramRequestMilliSecondInterval))
           .AddKeyedScoped<TelegramHttpHandler>(botSetting.BotName, (z, o) =>
           {
               var loggerFactory = z.GetRequiredService<ILoggerFactory>();

               return new(loggerFactory.CreateLogger<TelegramHttpHandler>(), z.GetRequiredKeyedService<IDispatcher>(botSetting.BotName), botSetting);
           })
           .AddKeyedScoped(typeof(ITelegramContextLog), botSetting.BotName, Log!);

        var policy = GetRetryPolicy(botSetting.HttpPolicy);

        var httpBuilder = serviceCollection.AddHttpClient(botSetting.BotName);

        if (HttpClient.IsNotNull())
        {
            httpBuilder.ConfigureHttpClient(HttpClient!);
        }

        httpBuilder
               .AddPolicyHandler(policy)
               .AddHttpMessageHandler(z => z.GetRequiredKeyedService<TelegramHttpHandler>(botSetting.BotName));

        serviceCollection.AddKeyedScoped<ITelegramContext, TelegramContext>(botSetting.BotName, (z, o) =>
        {
            var httpClientFactory = z.GetRequiredService<IHttpClientFactory>();
            var log = z.GetRequiredKeyedService<ITelegramContextLog>(botSetting.BotName);
            var mgr = z.GetRequiredService<RecyclableMemoryStreamManager>();

            return new(httpClientFactory.CreateClient(botSetting.BotName), botSetting, log, mgr);
        });
    }

    private static AsyncPolicy<HttpResponseMessage> GetRetryPolicy(TBotSettingHttpPolicy httpPolicy)
        => HttpPolicyExtensions
          .HandleTransientHttpError()
          .OrResult(response => IsRetryableStatusCode(httpPolicy, response))
          .WaitAndRetryAsync(
               httpPolicy.RetryCount,
               (_, response, _) => response?.Result?.Headers.RetryAfter?.Delta ?? TimeSpan.FromMilliseconds(httpPolicy.RetryMilliSecondInterval),
               (_, _, _, _) => Task.CompletedTask
               );

    /// <summary>
    /// Дополнительные (не transient) статусы, которые повторяет Polly.
    /// </summary>
    /// <remarks>
    /// 429 (TooManyRequests) намеренно исключён: его повторяет сам TelegramBotClient
    /// (TelegramBotClientOptions.RetryCount/RetryThreshold + RetryAfter), иначе один запрос
    /// уходит в сеть RetryCount * RetryCount раз.
    /// Пустой <see cref="TBotSettingHttpPolicy.BadStatuses"/> означает «повторять только transient-ошибки»:
    /// ответы 400/401/403/404 детерминированы и повтор их бессмысленен.
    /// </remarks>
    private static bool IsRetryableStatusCode(TBotSettingHttpPolicy httpPolicy, HttpResponseMessage response)
        => response.StatusCode != HttpStatusCode.TooManyRequests
           && httpPolicy.BadStatuses.IsNotNull()
           && ((int)response.StatusCode).In(httpPolicy.BadStatuses!);

    private static TimeLimiter GetLimeLimiter(int telegramRequestMilliSecondInterval)
        => TimeLimiter.GetFromMaxCountByInterval(RateContextConstant.MaxCountIteration, TimeSpan.FromMilliseconds(telegramRequestMilliSecondInterval));
}
