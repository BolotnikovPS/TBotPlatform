using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.BackgroundServices.Base;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Queues;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.BackgroundServices;

internal class TelegramDelayHostedService(ILogger<TelegramDelayHostedService> logger, IDelayQueue delayQueue, IServiceProvider services) : BackgroundServiceBase<TelegramDelayHostedService>
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var request = await delayQueue.Dequeue(stoppingToken);

            Exception? exception = null;
            try
            {
                var stateContextFactory = services.GetRequiredService<IStateContextFactory>();

                // StateContext owns its own AsyncServiceScope and must dispose of it,
                // otherwise the scope (together with scoped services and the HttpClient) leaks on every delayed request.
                await using var stateContextMinimal = stateContextFactory.GetStateContext(request.BotName, request.ChatId);

                await request.Value.Invoke(stateContextMinimal);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                var logLevel = exception.IsNotNull() ? LogLevel.Error : LogLevel.Debug;

                logger.Log(logLevel, exception, "Отправка сообщения с задержкой от бота {botName} в чат {chatId}", request.BotName, request.ChatId);
            }
        }
    }
}
