using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Queues;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Queues;

public interface IDelayQueue
{
    /// <summary>
    /// Добавляет сообщение в очередь
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="chatId">Id чата с которым будем взаимодействовать</param>
    /// <param name="delay">Время задержки отправки сообщения</param>
    /// <param name="item"></param>Запрос для отправки сообщения</param>
    void Enqueue(string botName, long chatId, TimeSpan delay, Func<IStateContextMinimal, Task<Message>> item);

    /// <summary>
    /// Получает сообщение из очереди
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DelayQueueItem> Dequeue(CancellationToken cancellationToken);
}
