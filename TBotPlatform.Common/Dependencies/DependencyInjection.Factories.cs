using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.State;
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
           .AddScoped<IStateFactory, StateFactory>()
           .AddScoped<IStateBind, StateFactory>()
           .AddScoped<IStateContextFactory, StateContextFactory>();

        return services;
    }
}