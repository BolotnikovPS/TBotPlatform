using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Queues;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.BackgroundServices;

internal class TelegramDelayHostedService(ILogger<TelegramDelayHostedService> logger, IDelayQueue delayQueue, IServiceProvider services) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var request = await delayQueue.Dequeue(stoppingToken);

            Exception exception = null;
            try
            {
                await using var scope = services.CreateAsyncScope();
                var stateContextFactory = scope.ServiceProvider.GetRequiredService<IStateContextFactory>();
                var stateContextMinimal = stateContextFactory.GetStateContext(request.BotName, request.ChatId);

                await request.Value.Invoke(stateContextMinimal);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                var logLevel = exception.IsNotNull() ? LogLevel.Error : LogLevel.Debug;

                logger.Log(logLevel, exception, "Отправка сообщения с задержкой от бота {botName} в чат {user}", request.BotName, request.ChatId);
            }
        }
    }
}
