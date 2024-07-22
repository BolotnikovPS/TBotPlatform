using System.Diagnostics;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    private static readonly SemaphoreSlim Semaphore = new(1);
    private static readonly Stopwatch Timer = new();

    private static int _iteration = 1;

    private const int IterationWaitSecond = 1 * 1000;

    public static async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator, CancellationToken cancellationToken)
    {
        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            Timer.Start();

            if (_iteration % 25 != 0
                && Timer.ElapsedMilliseconds <= IterationWaitSecond
               )
            {
                return await taskGenerator();
            }

            _iteration = 0;
            await Task.Delay(IterationWaitSecond, cancellationToken);
            Timer.Restart();

            return await taskGenerator();
        }
        finally
        {
            _iteration++;
            Timer.Stop();
            Semaphore.Release();
        }
    }

    public static async Task Enqueue(Func<Task> taskGenerator, CancellationToken cancellationToken)
    {
        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            Timer.Start();

            if (_iteration % 25 == 0
                || Timer.ElapsedMilliseconds > IterationWaitSecond
               )
            {
                _iteration = 0;
                await Task.Delay(IterationWaitSecond, cancellationToken);
                Timer.Restart();
            }

            await taskGenerator();
        }
        finally
        {
            _iteration++;
            Timer.Stop();
            Semaphore.Release();
        }
    }
}