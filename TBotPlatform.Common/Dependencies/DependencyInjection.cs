﻿using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Common.BackgroundServices;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddTelegramClient(this IServiceCollection services, string token)
    {
        if (!token.CheckAny())
        {
            throw new ArgumentNullException(nameof(token));
        }

        services
           .AddScoped<ITelegramBotClient, TelegramBotClient>(_ => new(token));

        return services;
    }

    public static IServiceCollection AddTelegramClientHostedService(this IServiceCollection services, TelegramSettings telegramSettings, params UpdateType[] updateType)
    {
        services
           .AddSingleton(
                new TelegramClientHostedServiceSettings
                {
                    UpdateType = updateType,
                })
           .AddSingleton(telegramSettings)
           .AddTelegramClient(telegramSettings.Token)
           .AddHostedService<TelegramClientHostedService>();

        return services;
    }

    public static IServiceCollection AddReceivingHandler<T>(this IServiceCollection services)
        where T : IStartReceivingHandler
    {
        services
           .AddScoped(typeof(IStartReceivingHandler), typeof(T));

        return services;
    }
}