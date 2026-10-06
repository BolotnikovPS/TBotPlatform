#nullable enable
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Handlers;

public interface IStartReceivingHandler
{
    /// <summary>
    /// Processes data when a request arrives from Telegram
    /// </summary>
    /// <param name="botName">Bot name</param>
    /// <param name="update">Update received from Telegram</param>
    /// <param name="markupNextState">Data from the inline button</param>
    /// <param name="telegramData">Telegram user and chat data</param>
    /// <param name="cancellationToken"></param>
    Task<IResult> HandleUpdate(string botName, Update update, MarkupNextState? markupNextState, TelegramMessageUserData telegramData, CancellationToken cancellationToken);
}