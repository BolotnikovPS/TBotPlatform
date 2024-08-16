using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TBotPlatform.Extension;

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
            Timer.Stop();

            await telegramContextLog.HandleEnqueueLogAsync(_iteration, Timer.Elapsed.Seconds, _operationGuid, cancellationToken);

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

        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            Timer.Start();

            await DelayAsync(cancellationToken);

            return await execute();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Исключение при работе с telegram. OperationGuid: {operationGuid}", _operationGuid.ToString());
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

        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            Timer.Start();

            await DelayAsync(cancellationToken);

            await execute();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Исключение при работе с telegram. OperationGuid: {operationGuid}", _operationGuid.ToString());
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