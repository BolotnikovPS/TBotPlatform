using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddFactories<T>(
        this IServiceCollection services,
        Assembly executingAssembly
        )
        where T : UserBase
    {
        services
           .AddSingleton(
                new StateFactorySettings
                {
                    Assembly = executingAssembly,
                })
           .AddScoped<IStateFactory<T>, StateFactory<T>>()
           .AddScoped<IStateContextFactory, StateContextFactory>();

        return services;
    }
}