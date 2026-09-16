#nullable enable
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Handlers;

public interface ITelegramUpdateProcessor
{
    /// <summary>
    /// Обрабатывает одно обновление. Можно вызывать из webhook.
    /// </summary>
    Task ProcessUpdate(string botName, Update update, CancellationToken cancellationToken);

    /// <summary>
    /// Обрабатывает пачку обновлений: разные чаты параллельно, один чат — последовательно.
    /// </summary>
    Task ProcessUpdates(string botName, IReadOnlyList<Update> updates, CancellationToken cancellationToken);
}
