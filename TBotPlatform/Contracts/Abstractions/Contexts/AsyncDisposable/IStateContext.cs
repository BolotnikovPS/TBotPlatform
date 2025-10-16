using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.States;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

public interface IStateContext : IStateContextMinimal
{
    /// <summary>
    /// Наименование бота
    /// </summary>
    string BotName { get; }

    /// <summary>
    /// Результат работы состояния
    /// </summary>
    StateResult StateResult { get; set; }

    /// <summary>
    /// Информация о сообщении из чата
    /// </summary>
    Update ChatUpdate { get; }

    /// <summary>
    /// Информация о состоянии входящей кнопки inline меню
    /// </summary>
    MarkupNextState MarkupNextState { get; }

    /// <summary>
    /// Осуществляет запрос в чат, отличающийся от заданного в <see cref="IStateContextFactory"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="newChatId">Id нового чата</param>
    /// <param name="request">Запрос для отправки сообщения</param>
    /// <returns></returns>
    Task<T> MakeRequestToOtherChat<T>(long newChatId, Func<IStateContextMinimal, Task<T>> request);

    /// <summary>
    /// Осуществляет запрос в чат с задержкой
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="timeSpan">Время задержки</param>
    /// <param name="request">Запрос для отправки сообщения</param>
    /// <returns></returns>
    void MakeDelayRequest(TimeSpan timeSpan, Func<IStateContextMinimal, Task<Message>> request);

    /// <summary>
    /// Устанавливает необходимость зафиксировать состояние
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task<IResult> BindState(CancellationToken cancellationToken);

    /// <summary>
    /// Снимает необходимость зафиксировать состояние
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult> UnBindState(CancellationToken cancellationToken);
}