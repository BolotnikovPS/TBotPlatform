using System.Diagnostics;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    private readonly Stopwatch _timer = new();
    private int _iteration;
    
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
        cancellationToken.ThrowIfCancellationRequested();
        
        if (execute.IsNull())
        {
            throw new ArgumentException(nameof(execute));
        }

        try
        {
            _timer.Start();

            return await execute();
        }
        finally
        {
            _iteration++;
            _timer.Stop();
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
        cancellationToken.ThrowIfCancellationRequested();

        if (execute.IsNull())
        {
            throw new ArgumentException(nameof(execute));
        }

        try
        {
            _timer.Start();

            await execute();
        }
        finally
        {
            _iteration++;
            _timer.Stop();
        }
    }
}