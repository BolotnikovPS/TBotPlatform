using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Contracts.Abstractions.Builder;

public interface IBotBuilder
{
    /// <summary>
    /// Добавляет контекст telegram
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    /// <param name="httpClient">Веб клиент</param>
    /// <returns></returns>
    IBotBuilder AddTelegramContext<TLog>(Action<HttpClient>? httpClient = null)
        where TLog : ITelegramContextLog;

    /// <summary>
    /// Добавляет контекст telegram
    /// </summary>
    /// <param name="httpClient">Веб клиент</param>
    /// <returns></returns>
    IBotBuilder AddTelegramContext(Action<HttpClient>? httpClient = null);

    /// <summary>
    /// Добавляет состояния
    /// </summary>
    /// <param name="executingAssembly">Сборка в которой находятся потенциальные состояния</param>
    /// <returns></returns>
    IBotBuilder AddStates(Assembly executingAssembly);

    /// <summary>
    /// Добавляет состояния
    /// </summary>
    /// <param name="potentialStateTypes">Список типов потенциальных состояний</param>
    /// <returns></returns>
    IBotBuilder AddStates(List<Type> potentialStateTypes);

    /// <summary>
    /// Добавляет обработчик событий от telegram
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
