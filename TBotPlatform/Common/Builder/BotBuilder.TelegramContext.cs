using ComposableAsync;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
           .AddKeyedSingleton(typeof(IDispatcher), telegramSettings.BotName, (z, s) => GetLimeLimiter(telegramSettings.HttpPolicy.TelegramRequestMilliSecondInterval))
           .AddKeyedScoped<TelegramHttpHandler>(telegramSettings.BotName, (z, o) =>
           {
               var loggerFactory = z.GetRequiredService<ILoggerFactory>();

               return new(loggerFactory.CreateLogger<TelegramHttpHandler>(), z.GetRequiredKeyedService<IDispatcher>(telegramSettings.BotName));
           })
           .AddKeyedScoped(typeof(ITelegramContextLog), telegramSettings.BotName, Log);

        var policy = GetRetryPolicy(telegramSettings.HttpPolicy);

        var httpBuilder = serviceCollection.AddHttpClient(telegramSettings.BotName);

        if (HttpClient.IsNotNull())
        {
            httpBuilder.ConfigureHttpClient(HttpClient!);
        }

        httpBuilder
               .AddPolicyHandler(policy)
               .AddHttpMessageHandler(z => z.GetRequiredKeyedService<TelegramHttpHandler>(telegramSettings.BotName));

        serviceCollection.AddKeyedScoped<ITelegramContext, TelegramContext>(telegramSettings.BotName, (z, o) =>
        {
            var httpClientFactory = z.GetRequiredService<IHttpClientFactory>();
            var log = z.GetRequiredKeyedService<ITelegramContextLog>(telegramSettings.BotName);
            return new TelegramContext(httpClientFactory.CreateClient(telegramSettings.BotName), telegramSettings, log);
        });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(TelegramSettingsHttpPolicy httpPolicy)
        => HttpPolicyExtensions
          .HandleTransientHttpError()
          .OrResult(
               res =>
                   res.Headers.IsNotNull()
                   && httpPolicy.BadStatuses.IsNull()
                       ? res.StatusCode.NotIn(HttpStatusCode.OK, HttpStatusCode.NoContent)
                       : ((int)res.StatusCode).In(httpPolicy.BadStatuses)
               )
          .WaitAndRetryAsync(
               httpPolicy.RetryCount,
               (_, response, _) => response?.Result?.Headers.RetryAfter?.Delta ?? TimeSpan.FromMilliseconds(httpPolicy.RetryMilliSecondInterval),
               (_, _, _, _) => Task.CompletedTask
               );

    private static IDispatcher GetLimeLimiter(int telegramRequestMilliSecondInterval)
        => TimeLimiter.GetFromMaxCountByInterval(RateContextConstant.MaxCountIteration, TimeSpan.FromMilliseconds(telegramRequestMilliSecondInterval));
}
