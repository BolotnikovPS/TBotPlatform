using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Factories;
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
           .AddScoped<IMenuButtonFactory, MenuButtonFactory>();

        return services;
    }
}