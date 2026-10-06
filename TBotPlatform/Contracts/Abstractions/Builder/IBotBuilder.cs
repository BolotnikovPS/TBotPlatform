#nullable enable
using System.Reflection;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Handlers;

namespace TBotPlatform.Contracts.Abstractions.Builder;

public interface IBotBuilder
{
    /// <summary>
    /// Adds the Telegram context <see cref="ITelegramContext"/>
    /// </summary>
    /// <param name="httpClient">HTTP client</param>
    IBotBuilder AddTelegramContext<TLog>(Action<HttpClient>? httpClient = null)
        where TLog : ITelegramContextLog;

    /// <summary>
    /// Adds the Telegram context <see cref="ITelegramContext"/>
    /// </summary>
    /// <param name="httpClient">HTTP client</param>
    IBotBuilder AddTelegramContext(Action<HttpClient>? httpClient = null);

    /// <summary>
    /// Adds the states <see cref="IStateFactory"/>, <see cref="IStateBindFactory"/>, <see cref="IStateContextFactory"/>
    /// </summary>
    /// <param name="executingAssembly">Assembly that contains the potential states</param>
    IBotBuilder AddStates(Assembly executingAssembly);

    /// <summary>
    /// Adds the states <see cref="IStateFactory"/>, <see cref="IStateBindFactory"/>, <see cref="IStateContextFactory"/>
    /// </summary>
    /// <param name="potentialStateTypes">List of potential state types</param>
    IBotBuilder AddStates(List<Type> potentialStateTypes);

    /// <summary>
    /// Adds the event handler from Telegram <see cref="IStartReceivingHandler"/>
    /// </summary>
    IBotBuilder AddReceivingHandler<T>()
        where T : IStartReceivingHandler;

    /// <summary>
    /// Builds the bot
    /// </summary>
    IBotPlatformBuilder Build();
}
