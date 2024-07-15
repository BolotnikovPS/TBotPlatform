namespace TBotPlatform.Common.Contexts;

internal class TelegramContextTaskQueue
{
    private readonly SemaphoreSlim _semaphore = new(1);

    private static int _iteration = 1;
    private const int IterationWaitSecond = 1 * 1000;

    public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_iteration % 25 != 0)
            {
                return await taskGenerator();
            }

            _iteration = 0;
            await Task.Delay(IterationWaitSecond, cancellationToken);

            return await taskGenerator();
        }
        finally
        {
            _iteration++;
            _semaphore.Release();
        }
    }

    public async Task Enqueue(Func<Task> taskGenerator, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_iteration % 25 == 0)
            {
                _iteration = 0;
                await Task.Delay(IterationWaitSecond, cancellationToken);
            }

            await taskGenerator();
        }
        finally
        {
            _iteration++;
            _semaphore.Release();
        }
    }
}