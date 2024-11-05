#nullable enable
using ComposableAsync;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter;
using System.Reflection;
using TBotPlatform.Common.Factories;
using TBotPlatform.Common.Factories.Proxies;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Factories.Proxies;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.Constant;
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
           .AddSingleton(
                new StateFactorySettings
                {
                    Assembly = executingAssembly,
                })
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
        var policy = GetRetryPolicy(telegramSettingsHttpPolicy ?? new());

        services
           .AddScoped<LoggingHttpHandler>()
           .AddScoped(typeof(ITelegramContextLog), typeof(TLog));

        var limiterHandler = TimeLimiter
                            .GetFromMaxCountByInterval(RateContextConstant.MaxCountIteration, TimeSpan.FromSeconds(1))
                            .AsDelegatingHandler();

        if (httpClient.IsNotNull())
        {
            services
               .AddHttpClient<ITelegramContextProxyFactory, TelegramContextProxyFactory>(httpClient!)
               .AddHttpMessageHandler<LoggingHttpHandler>()
               .AddHttpMessageHandler(() => limiterHandler)
               .AddPolicyHandler(policy);
        }
        else
        {
            services
               .AddHttpClient<ITelegramContextProxyFactory, TelegramContextProxyFactory>()
               .AddHttpMessageHandler<LoggingHttpHandler>()
               .AddHttpMessageHandler(() => limiterHandler)
               .AddPolicyHandler(policy);
        }

        return services;
    }

    public static IServiceCollection AddProxyFactories(this IServiceCollection services)
    {
        services
           .AddScoped<IStateContextProxyFactory, StateContextProxyFactory>();

        return services;
    }
}