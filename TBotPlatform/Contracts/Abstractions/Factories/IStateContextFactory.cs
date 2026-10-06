#nullable enable
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Users;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Factories;

public interface IStateContextFactory
{
    /// <summary>
    /// Creates a state context. The base chat is the user's chat.
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="user">User to interact with</param>
    IStateContextMinimal GetStateContext<T>(string botName, T user) where T : UserBase;

    /// <summary>
    /// Creates a state context
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="chatId">Chat id to interact with</param>
    IStateContextMinimal GetStateContext(string botName, long chatId);

    /// <summary>
    /// Creates a state context and invokes a state. The base chat is the user's chat.
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="user">User to interact with</param>
    /// <param name="stateHistory">State being opened</param>
    /// <param name="update">Update received from Telegram</param>
    /// <param name="cancellationToken"></param>
    Task<IStateContextMinimal> CreateStateContext<T>(string botName, T user, StateHistory stateHistory, Update update, CancellationToken cancellationToken)
        where T : UserBase;

    /// <summary>
    /// Creates a state context and invokes a state. The base chat is the user's chat.
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="user">User to interact with</param>
    /// <param name="stateHistory">State being opened</param>
    /// <param name="update">Update received from Telegram</param>
    /// <param name="markupNextState">Data from the inline button</param>
    /// <param name="cancellationToken"></param>
    Task<IStateContextMinimal> CreateStateContext<T>(
        string botName,
        T user,
        StateHistory stateHistory,
        Update update,
        MarkupNextState? markupNextState,
        CancellationToken cancellationToken
        )
        where T : UserBase;
}