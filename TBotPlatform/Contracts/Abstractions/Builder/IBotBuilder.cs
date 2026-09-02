#nullable enable
using System.Reflection;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Handlers;

namespace TBotPlatform.Contracts.Abstractions.Builder;

public interface IBotBuilder
{
    /// <summary>
    /// Добавляет контекст telegram <see cref="ITelegramContext"/>
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    /// <param name="httpClient">Веб клиент</param>
    /// <returns></returns>
    IBotBuilder AddTelegramContext<TLog>(Action<HttpClient>? httpClient = null)
        where TLog : ITelegramContextLog;

    /// <summary>
    /// Добавляет контекст telegram <see cref="ITelegramContext"/>
    /// </summary>
    /// <param name="httpClient">Веб клиент</param>
    /// <returns></returns>
    IBotBuilder AddTelegramContext(Action<HttpClient>? httpClient = null);

    /// <summary>
    /// Добавляет состояния <see cref="IStateFactory"/>, <see cref="IStateBindFactory"/>, <see cref="IStateContextFactory"/>
    /// </summary>
    /// <param name="executingAssembly">Сборка в которой находятся потенциальные состояния</param>
    /// <returns></returns>
    IBotBuilder AddStates(Assembly executingAssembly);

    /// <summary>
    /// Добавляет состояния <see cref="IStateFactory"/>, <see cref="IStateBindFactory"/>, <see cref="IStateContextFactory"/>
    /// </summary>
    /// <param name="potentialStateTypes">Список типов потенциальных состояний</param>
    /// <returns></returns>
    IBotBuilder AddStates(List<Type> potentialStateTypes);

    /// <summary>
    /// Добавляет обработчик событий от telegram <see cref="IStartReceivingHandler"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IBotBuilder AddReceivingHandler<T>()
        where T : IStartReceivingHandler;

    /// <summary>
    /// Собирает бота
    /// </summary>
    /// <returns></returns>
    IBotPlatformBuilder Build();
}
