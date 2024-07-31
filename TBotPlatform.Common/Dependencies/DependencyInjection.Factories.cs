using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Factories;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddFactories(this IServiceCollection services, Assembly executingAssembly)
    {
        services
           .AddSingleton(
                new StateFactorySettings
                {
                    Assembly = executingAssembly,
                })
           .AddScoped<IStateFactory, StateFactory>()
           .AddScoped<IStateContextFactory, StateContextFactory>();

        return services;
    }
}