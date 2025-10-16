using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Contracts.Abstractions.Builder;

public interface IBotPlatformBuilder
{
    /// <summary>
    /// Добавляет бота
    /// </summary>
    /// <param name="telegramSettings">Настройки контекста бота</param>
    /// <returns></returns>
    IBotBuilder AddBot(TelegramSettings telegramSettings);

    /// <summary>
    /// Добавляет кеш для работы ботов
    /// </summary>
    /// <returns></returns>
    ICacheBuilder AddCache();

    /// <summary>
    /// Добавляет фоновый сервиса для получения обновлений telegram
    /// </summary>
    /// <returns></returns>
    IBotPlatformBuilder AddHostedService();

    /// <summary>
    /// Добавляет фабрики для работы ботов <see cref="IStateFactory"/>, <see cref="IStateBindFactory"/>, <see cref="IStateContextFactory"/>, <see cref="IMenuButtonFactory"/>
    /// </summary>
    /// <param name="executingAssembly">Сборка в которой находятся потенциальные состояния</param>
    /// <returns></returns>
    IBotPlatformBuilder AddFactories(Assembly executingAssembly);

    /// <summary>
    /// Собирает платформу
    /// </summary>
    /// <returns></returns>
    IServiceCollection Build();
}
