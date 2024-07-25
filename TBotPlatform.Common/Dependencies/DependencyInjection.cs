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
    public static IServiceCollection AddTelegramContext<T>(this IServiceCollection services, TelegramSettings telegramSettings)
        where T : ITelegramStatisticContext
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
           .AddScoped(typeof(ITelegramStatisticContext), typeof(T))
           .AddHttpClient<ITelegramContext, TelegramContext>()
           .AddHttpMessageHandler<LoggingHttpHandler>()
           .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    public static IServiceCollection AddTelegramContext(this IServiceCollection services, TelegramSettings telegramSettings)
        => services.AddTelegramContext<TelegramStatisticContext>(telegramSettings);

    public static IServiceCollection AddTelegramClientHostedService<T>(this IServiceCollection services, TelegramSettings telegramSettings, params UpdateType[] updateType)
        where T : ITelegramStatisticContext
    {
        services
           .AddSingleton(
                new TelegramContextHostedServiceSettings
                {
                    UpdateType = updateType,
                })
           .AddTelegramContext<T>(telegramSettings)
           .AddHostedService<TelegramContextHostedService>();

        return services;
    }

    public static IServiceCollection AddTelegramContextHostedService(this IServiceCollection services, TelegramSettings telegramSettings, params UpdateType[] updateType)
        => services.AddTelegramClientHostedService<TelegramStatisticContext>(telegramSettings, updateType);

    public static IServiceCollection AddReceivingHandler<T>(this IServiceCollection services)
        where T : IStartReceivingHandler
    {
        services
           .AddScoped(typeof(IStartReceivingHandler), typeof(T));

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
              .OrResult(
                   res =>
                       res.Headers.IsNotNull()
                       && res.Headers.RetryAfter.IsNotNull()
                   )
              .WaitAndRetryAsync(
                   3,
                   (_, response, _) => response.Result.Headers.RetryAfter.Delta.Value,
                   (_, _, _, _) => Task.CompletedTask
                   );
    }
}