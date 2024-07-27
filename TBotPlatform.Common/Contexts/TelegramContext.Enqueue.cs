using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    private static readonly SemaphoreSlim Semaphore = new(1);
    private static readonly Stopwatch Timer = new();

    private static int _iteration = 1;

    private const int IterationWaitMilliSecond = 1 * 1000;
    private const int MaxCountIteration = 29;

    /// <summary>
    /// Делает задержку при превышении лимита отправки сообщений в telegram
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task DelayAsync(CancellationToken cancellationToken)
    {
        if (_iteration > MaxCountIteration)
        {
            logger.LogDebug("Отправлено {count} запросов в telegram за {second} секунд", _iteration, Timer.Elapsed.Seconds);

            if (Timer.ElapsedMilliseconds <= IterationWaitMilliSecond)
            {
                await Task.Delay(IterationWaitMilliSecond, cancellationToken);
            }

            _iteration = 0;
            Timer.Restart();
        }
    }

    /// <summary>
    /// Исполнение метода в очереди
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="taskGenerator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator, CancellationToken cancellationToken)
    {
        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            Timer.Start();

            await DelayAsync(cancellationToken);

            return await taskGenerator();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Исключение при работе с telegram");
            throw;
        }
        finally
        {
            _iteration++;
            Timer.Stop();
            Semaphore.Release();
        }
    }

    /// <summary>
    /// Исполнение метода в очереди
    /// </summary>
    /// <param name="taskGenerator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Enqueue(Func<Task> taskGenerator, CancellationToken cancellationToken)
    {
        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            Timer.Start();

            await DelayAsync(cancellationToken);

            await taskGenerator();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Исключение при работе с telegram");
            throw;
        }
        finally
        {
            _iteration++;
            Timer.Stop();
            Semaphore.Release();
        }
    }
}