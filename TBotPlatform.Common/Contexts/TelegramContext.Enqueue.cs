using System.Diagnostics;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    private static readonly Stopwatch Timer = new();

    private static int _iteration = 1;

    /// <summary>
    /// Делает задержку при превышении лимита отправки сообщений в telegram
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task DelayAsync(CancellationToken cancellationToken)
    {
        if (_iteration > RateContextConstant.MaxCountIteration)
        {
            Timer.Stop();

            await telegramContextLog.HandleEnqueueLogAsync(_iteration, Timer.Elapsed.Seconds, _operationGuid, cancellationToken);

            _iteration = 0;
            Timer.Restart();
        }
    }

    /// <summary>
    /// Исполнение метода в очереди
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="execute">Исполняемая функция</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private async Task<T> Enqueue<T>(Func<Task<T>> execute, CancellationToken cancellationToken)
    {
        if (execute.IsNull())
        {
            throw new ArgumentException(nameof(execute));
        }

        try
        {
            Timer.Start();

            await DelayAsync(cancellationToken);

            return await execute();
        }
        finally
        {
            _iteration++;
            Timer.Stop();
        }
    }

    /// <summary>
    /// Исполнение метода в очереди
    /// </summary>
    /// <param name="execute">Исполняемая функция</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private async Task Enqueue(Func<Task> execute, CancellationToken cancellationToken)
    {
        if (execute.IsNull())
        {
            throw new ArgumentException(nameof(execute));
        }

        try
        {
            Timer.Start();

            await DelayAsync(cancellationToken);

            await execute();
        }
        finally
        {
            _iteration++;
            Timer.Stop();
        }
    }
}