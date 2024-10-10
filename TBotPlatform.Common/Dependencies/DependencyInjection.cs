#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using TBotPlatform.Common.BackgroundServices;
using TBotPlatform.Common.Contexts;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddTelegramContext<TLog>(this IServiceCollection services, TelegramSettings telegramSettings)
        where TLog : ITelegramContextLog
    {
        if (telegramSettings.IsNull() || telegramSettings.Token.IsNull())
        {
            throw new ArgumentNullException(nameof(TelegramSettings.Token));
        }

        var policy = GetRetryPolicy(telegramSettings.HttpPolicy);

        services
           .AddScoped<LoggingHttpHandler>()
           .AddSingleton(telegramSettings)
           .AddScoped(typeof(ITelegramContextLog), typeof(TLog))
           .AddHttpClient<ITelegramContext, TelegramContext>()
           .AddHttpMessageHandler<LoggingHttpHandler>()
           .AddPolicyHandler(policy);

        services
           .AddScoped<TelegramChatHandler>()
           .AddScoped<ITelegramUpdateHandler>(src => src.GetRequiredService<TelegramChatHandler>())
           .AddScoped<ITelegramMappingHandler>(src => src.GetRequiredService<TelegramChatHandler>());

        return services;
    }

    public static IServiceCollection AddTelegramContext(this IServiceCollection services, TelegramSettings telegramSettings)
        => services.AddTelegramContext<TelegramContextLog>(telegramSettings);

    public static IServiceCollection AddTelegramClientHostedService<TLog>(this IServiceCollection services, TelegramSettings telegramSettings)
        where TLog : ITelegramContextLog
    {
        services
           .AddTelegramContext<TLog>(telegramSettings)
           .AddHostedService<TelegramContextHostedService>();

        return services;
    }

    public static IServiceCollection AddTelegramContextHostedService(this IServiceCollection services, TelegramSettings telegramSettings)
        => services.AddTelegramClientHostedService<TelegramContextLog>(telegramSettings);

    public static IServiceCollection AddReceivingHandler<T>(this IServiceCollection services)
        where T : IStartReceivingHandler
    {
        services
           .AddScoped(typeof(IStartReceivingHandler), typeof(T));

        return services;
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
}