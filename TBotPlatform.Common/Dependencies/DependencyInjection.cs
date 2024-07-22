using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Common.BackgroundServices;
using TBotPlatform.Common.Contexts;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Extension;
using Telegram.Bot;
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
           .AddScoped<ITelegramBotClient, TelegramBotClient>(_ => new(telegramSettings.Token))
           .AddSingleton(telegramSettings)
           .AddScoped(typeof(ITelegramStatisticContext), typeof(T))
           .AddScoped<ITelegramContext, TelegramContext>();

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
}