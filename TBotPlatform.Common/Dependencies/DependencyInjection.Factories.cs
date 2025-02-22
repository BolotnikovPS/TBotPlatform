﻿#nullable enable
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Common.Contexts;
using TBotPlatform.Common.Contracts;
using TBotPlatform.Common.Factories;
using TBotPlatform.Common.Factories.Proxies;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Factories.Proxies;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddFactories(this IServiceCollection services, Assembly executingAssembly)
    {
        if (executingAssembly.IsNull())
        {
            throw new ArgumentNullException(nameof(executingAssembly));
        }

        services
           .AddOptions<StateFactorySettings>()
           .Configure(option => { option.Assembly = executingAssembly; });

        services
           .AddScoped<StateFactory>()
           .AddScoped<IStateFactory>(src => src.GetRequiredService<StateFactory>())
           .AddScoped<IStateBindFactory>(src => src.GetRequiredService<StateFactory>())
           .AddScoped<IStateContextFactory, StateContextFactory>()
           .AddSingleton<IMenuButtonFactory, MenuButtonFactory>();

        return services;
    }

    public static IServiceCollection AddTelegramContextProxyFactory<TLog>(
        this IServiceCollection services,
        TelegramSettingsHttpPolicy? telegramSettingsHttpPolicy = null,
        Action<HttpClient>? httpClient = null
        )
        where TLog : ITelegramContextLog
    {
        telegramSettingsHttpPolicy ??= new();
        var policy = GetRetryPolicy(telegramSettingsHttpPolicy);

        services
           .AddSingleton(_ => GetLimeLimiter(telegramSettingsHttpPolicy.TelegramRequestMilliSecondInterval))
           .AddScoped<TelegramHttpHandler>()
           .AddScoped(typeof(ITelegramContextLog), typeof(TLog));

        if (httpClient.IsNotNull())
        {
            services
               .AddHttpClient<ITelegramContextProxyFactory, TelegramContextProxyFactory>(nameof(TelegramContextProxyFactory), httpClient!)
               .AddHttpMessageHandler<TelegramHttpHandler>()
               .AddPolicyHandler(policy);
        }
        else
        {
            services
               .AddHttpClient<ITelegramContextProxyFactory, TelegramContextProxyFactory>(nameof(TelegramContextProxyFactory))
               .AddHttpMessageHandler<TelegramHttpHandler>()
               .AddPolicyHandler(policy);
        }

        return services;
    }

    public static IServiceCollection AddTelegramContextProxyFactory(
        this IServiceCollection services,
        TelegramSettingsHttpPolicy? telegramSettingsHttpPolicy = null,
        Action<HttpClient>? httpClient = null
        )
        => services.AddTelegramContextProxyFactory<TelegramContextLog>(telegramSettingsHttpPolicy, httpClient);

    public static IServiceCollection AddProxyFactories(this IServiceCollection services)
        => services
          .AddScoped<IStateContextProxyFactory, StateContextProxyFactory>()
          .AddScoped<IStateProxyFactory, StateProxyFactory>();
}