using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.States;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

public interface IStateContext : IStateContextMinimal
{
    /// <summary>
    /// Bot name.
    /// </summary>
    string BotName { get; }

    /// <summary>
    /// Result of the state work.
    /// </summary>
    StateResult StateResult { get; set; }

    /// <summary>
    /// Information about a chat message.
    /// </summary>
    Update? ChatUpdate { get; }

    /// <summary>
    /// Information about the incoming inline menu button state.
    /// </summary>
    MarkupNextState? MarkupNextState { get; }

    /// <summary>
    /// Returns a <c>https://t.me/...</c> link to the message the request came from.
    /// For private chats and regular groups the link is unavailable — returns null.
    /// </summary>
    string? GetMessageLink();

    /// <summary>
    /// Makes a request to a chat other than the one specified in <see cref="IStateContextFactory"/>.
    /// </summary>
    /// <param name="newChatId">ID of the new chat</param>
    /// <param name="request">Request to send a message</param>
    Task<T> MakeRequestToOtherChat<T>(long newChatId, Func<IStateContextMinimal, Task<T>> request);

    /// <summary>
    /// Makes a delayed request to a chat.
    /// </summary>
    /// <param name="timeSpan">Delay time</param>
    /// <param name="request">Request to send a message</param>
    IResult MakeDelayRequest(TimeSpan timeSpan, Func<IStateContextMinimal, Task<Message>> request);

    /// <summary>
    /// Marks the state as needing to be bound.
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task<IResult> BindState(CancellationToken cancellationToken);

    /// <summary>
    /// Clears the mark that the state must be bound.
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task<IResult> UnBindState(CancellationToken cancellationToken);
}