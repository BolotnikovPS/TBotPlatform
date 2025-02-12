#nullable enable
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable.Proxies;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Contracts.State;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Factories.Proxies;

public interface IStateContextProxyFactory
{
    /// <summary>
    /// Создание контекста состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <param name="telegramContextProxy"></param>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <returns></returns>
    IStateContextProxyMinimal GetStateContext<T>(ITelegramContextProxy telegramContextProxy, T user)
        where T : UserBase;

    /// <summary>
    /// Создание контекста состояния
    /// </summary>
    /// <param name="telegramContextProxy"></param>
    /// <param name="chatId">Id чата с которым будем взаимодействовать</param>
    /// <returns></returns>
    IStateContextProxyMinimal GetStateContext(ITelegramContextProxy telegramContextProxy, long chatId);

    /// <summary>
    /// Создание контекста состояния и вызов состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <param name="telegramContextProxy"></param>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="update">Сообщение с telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContextProxyMinimal> CreateStateContext<T>(
        ITelegramContextProxy telegramContextProxy,
        T user,
        StateHistory stateHistory,
        Update update,
        CancellationToken cancellationToken
        )
        where T : UserBase;

    /// <summary>
    /// Создание контекста состояния и вызов состояния. Базовым чатом указывается чат пользователя.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="telegramContextProxy"></param>
    /// <param name="user">Пользователь с которым будем взаимодействовать</param>
    /// <param name="stateHistory">Вызываемое состояние</param>
    /// <param name="chatUpdate">Форматированное сообщение с telegram</param>
    /// <param name="markupNextState">Данные с кнопки inline</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContextProxyMinimal> CreateStateContext<T>(
        ITelegramContextProxy telegramContextProxy,
        T user,
        StateHistory stateHistory,
        Update chatUpdate,
        MarkupNextState? markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase;

    /// <summary>
    /// Получает результат выполнения состояния
    /// </summary>
    /// <param name="stateContext">Контекст состояния</param>
    /// <returns></returns>
    StateResult? GetStateResult(IStateContextProxyMinimal stateContext);
}