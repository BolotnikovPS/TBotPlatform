using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateContextFactory
{
    /// <summary>
    /// Создание контекста состояния и вызов самого состояния если оно имеется
    /// </summary>
    /// <param name="user"></param>
    /// <param name="stateHistory"></param>
    /// <param name="update"></param>
    /// <param name="markupNextState"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IStateContext> CreateStateContextAsync<T>(
        T user,
        StateHistory<T> stateHistory,
        Update update,
        MarkupNextState markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase;
}