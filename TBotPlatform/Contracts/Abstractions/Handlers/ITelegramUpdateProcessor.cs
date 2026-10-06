#nullable enable
using TBotPlatform.Results.Abstractions;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Handlers;

public interface ITelegramUpdateProcessor
{
    /// <summary>
    /// Processes a single update. Can be called from a webhook.
    /// Returns <see cref="IResult"/>. On failure, the webhook endpoint must return a non-2xx HTTP status to cause Telegram to retry delivery.
    /// </summary>
    Task<IResult> ProcessUpdate(string botName, Update update, CancellationToken cancellationToken);

    /// <summary>
    /// Processes a batch of updates: different chats in parallel, one chat sequentially.
    /// </summary>
    Task ProcessUpdates(string botName, IReadOnlyList<Update> updates, CancellationToken cancellationToken);
}
