using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Contracts.Abstractions.Builder;

public interface IBotPlatformBuilder
{
    /// <summary>
    /// Adds a bot <see cref="IBotBuilder"/>
    /// </summary>
    /// <param name="botSetting">Bot context settings</param>
    IBotBuilder AddBot(TBotSetting botSetting);

    /// <summary>
    /// Adds a cache for bot operation <see cref="ICacheBuilder"/>
    /// </summary>
    ICacheBuilder AddCache();

    /// <summary>
    /// Adds a background service for receiving Telegram updates
    /// </summary>
    IBotPlatformBuilder AddHostedService();

    /// <summary>
    /// Adds factories for bot operation <see cref="IStateFactory"/>, <see cref="IStateBindFactory"/>, <see cref="IStateContextFactory"/>, <see cref="IMenuButtonFactory"/>
    /// </summary>
    /// <param name="executingAssembly">Assembly that contains the potential states</param>
    IBotPlatformBuilder AddFactories(Assembly executingAssembly);

    /// <summary>
    /// Builds the platform
    /// </summary>
    IServiceCollection Build();
}
