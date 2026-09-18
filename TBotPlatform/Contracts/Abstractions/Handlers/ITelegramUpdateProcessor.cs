#nullable enable
using TBotPlatform.Results.Abstractions;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Handlers;

public interface ITelegramUpdateProcessor
{
    /// <summary>
    /// Обрабатывает одно обновление. Можно вызывать из webhook.
    /// Возвращает <see cref="IResult"/>. При неудаче webhook-эндпоинт должен вернуть Telegram не-2xx HTTP-статус, чтобы Telegram повторил доставку.
    /// </summary>
    Task<IResult> ProcessUpdate(string botName, Update update, CancellationToken cancellationToken);

    /// <summary>
    /// Обрабатывает пачку обновлений: разные чаты параллельно, один чат — последовательно.
    /// </summary>
    Task ProcessUpdates(string botName, IReadOnlyList<Update> updates, CancellationToken cancellationToken);
}
