#nullable enable
using ComposableAsync;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using RateLimiter;
using System.Net;
using TBotPlatform.Common.BackgroundServices;
using TBotPlatform.Common.Contexts;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddTelegramContext<TLog>(this IServiceCollection services, TelegramSettings telegramSettings, Action<HttpClient>? httpClient = null)
        where TLog : ITelegramContextLog
    {
        if (telegramSettings.IsNull())
        {
            throw new ArgumentNullException(nameof(telegramSettings));
        }

        services
           .AddSingleton(telegramSettings)
           .AddSingleton(_ => GetLimeLimiter(telegramSettings.HttpPolicy.TelegramRequestMilliSecondInterval))
           .AddScoped<LoggingHttpHandler>()
           .AddScoped(typeof(ITelegramContextLog), typeof(TLog));

        var policy = GetRetryPolicy(telegramSettings.HttpPolicy);

        if (httpClient.IsNotNull())
        {
            services
               .AddHttpClient<ITelegramContext, TelegramContext>(nameof(TelegramContext))
               .ConfigureHttpClient(httpClient!)
               .AddPolicyHandler(policy)
               .AddHttpMessageHandler<LoggingHttpHandler>();
        }
        else
        {
            services
               .AddHttpClient<ITelegramContext, TelegramContext>(nameof(TelegramContext))
               .AddPolicyHandler(policy)
               .AddHttpMessageHandler<LoggingHttpHandler>();
        }

        return services;
    }

    public static IServiceCollection AddTelegramContext(this IServiceCollection services, TelegramSettings telegramSettings, Action<HttpClient>? httpClient = null)
        => services.AddTelegramContext<TelegramContextLog>(telegramSettings, httpClient);

    public static IServiceCollection AddTelegramClientHostedService<TLog>(this IServiceCollection services, TelegramSettings telegramSettings, Action<HttpClient>? httpClient = null)
        where TLog : ITelegramContextLog
        => services
          .AddTelegramContext<TLog>(telegramSettings, httpClient)
          .AddHostedService<TelegramContextHostedService>();

    public static IServiceCollection AddTelegramContextHostedService(this IServiceCollection services, TelegramSettings telegramSettings, Action<HttpClient>? httpClient = null)
        => services.AddTelegramClientHostedService<TelegramContextLog>(telegramSettings, httpClient);

    public static IServiceCollection AddReceivingHandler<T>(this IServiceCollection services)
        where T : IStartReceivingHandler
        => services
           .AddScoped(typeof(IStartReceivingHandler), typeof(T));

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