#nullable enable
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Common.Contracts;
using TBotPlatform.Common.Factories;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    internal static IServiceCollection AddFactories(this IServiceCollection services, Assembly executingAssembly)
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
}