using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using TBotPlatform.Common.BackgroundServices;
using TBotPlatform.Common.Contexts;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Extension;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddTelegramContext<TLog>(this IServiceCollection services, TelegramSettings telegramSettings)
        where TLog : ITelegramContextLog
    {
        if (telegramSettings.IsNull()
            || telegramSettings.Token.IsNull()
           )
        {
            throw new ArgumentNullException(nameof(TelegramSettings.Token));
        }

        services
           .AddScoped<LoggingHttpHandler>()
           .AddSingleton(telegramSettings)
           .AddScoped(typeof(ITelegramContextLog), typeof(TLog))
           .AddHttpClient<ITelegramContext, TelegramContext>()
           .AddHttpMessageHandler<LoggingHttpHandler>()
           .AddPolicyHandler(GetRetryPolicy());

        services
           .AddScoped<ITelegramUpdateHandler, TelegramUpdateHandler>();

        return services;
    }

    public static IServiceCollection AddTelegramContext(this IServiceCollection services, TelegramSettings telegramSettings)
        => services.AddTelegramContext<TelegramContextLog>(telegramSettings);

    public static IServiceCollection AddTelegramClientHostedService<TLog>(this IServiceCollection services, TelegramSettings telegramSettings, params UpdateType[] updateType)
        where TLog : ITelegramContextLog
    {
        services
           .AddSingleton(
                new TelegramContextHostedServiceSettings
                {
                    UpdateType = updateType,
                })
           .AddTelegramContext<TLog>(telegramSettings)
           .AddHostedService<TelegramContextHostedService>();

        return services;
    }

    public static IServiceCollection AddTelegramContextHostedService(this IServiceCollection services, TelegramSettings telegramSettings, params UpdateType[] updateType)
        => services.AddTelegramClientHostedService<TelegramContextLog>(telegramSettings, updateType);

    public static IServiceCollection AddReceivingHandler<T>(this IServiceCollection services)
        where T : IStartReceivingHandler
    {
        services
           .AddScoped(typeof(IStartReceivingHandler), typeof(T));

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        => HttpPolicyExtensions
          .HandleTransientHttpError()
          .OrResult(
               res => res.StatusCode == HttpStatusCode.TooManyRequests
                      && res.Headers.IsNotNull()
                      && res.Headers.RetryAfter.IsNotNull()
               )
          .WaitAndRetryAsync(
               3,
               (_, response, _) => response.Result.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(1),
               (_, _, _, _) => Task.CompletedTask
               );
}