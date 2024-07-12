using Microsoft.Extensions.Logging;
using TBotPlatform.Extension;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext<T>
{
    private async Task ExecuteTaskAsync(Task method, CancellationToken cancellation)
    {
        try
        {
            await method;
            return;
        }
        catch (ApiRequestException ex)
        {
            var delay = ex.Parameters.CheckAny()
                        && ex.Parameters.RetryAfter.HasValue
                ? ex.Parameters.RetryAfter.Value
                : 100;

            logger.LogInformation("Ошибка. Задержка {delay}. Cообщение: {error}", delay, ex.ToJson());

            await Task.Delay(delay, cancellation);
        }

        await method;
    }

    private async Task<Message> ExecuteTaskAsync(Task<Message> method, CancellationToken cancellation)
    {
        try
        {
            await method;
        }
        catch (ApiRequestException ex)
        {
            var delay = ex.Parameters.CheckAny()
                        && ex.Parameters.RetryAfter.HasValue
                ? ex.Parameters.RetryAfter.Value
                : 100;

            logger.LogInformation("Ошибка. Задержка {delay}. Cообщение: {error}", delay, ex.ToJson());

            await Task.Delay(delay, cancellation);
        }

        return await method;
    }
}