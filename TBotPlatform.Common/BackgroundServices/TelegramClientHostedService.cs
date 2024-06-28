using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.BackgroundServices;

internal class TelegramClientHostedService(
    ILogger<TelegramClientHostedService> logger,
    TelegramClientHostedServiceSettings settings,
    ITelegramBotClient telegramBotClient,
    IServiceProvider services
    ) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await telegramBotClient.GetMeAsync(stoppingToken);

            logger.LogInformation("Запущен бот {name}", result.FirstName);

            ReceiverOptions receiverOptions = null;

            if (settings.UpdateType.CheckAny())
            {
                receiverOptions = new()
                {
                    AllowedUpdates = settings.UpdateType,
                };
            }

            await telegramBotClient.ReceiveAsync(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                stoppingToken
                );
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        logger.LogInformation("Поступило сообщение id = {updateId}: {update}", update.Id, update.ToJson());

        var timer = Stopwatch.StartNew();

        await using var scope = services.CreateAsyncScope();
        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IStartReceivingHandler>();

        await scopedProcessingService.HandleUpdateAsync(update, cancellationToken);

        timer.Stop();

        logger.LogInformation("Время выполнения для id = {updateId}: {elapsed}", update.Id, timer.Elapsed);
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Возникло исключение");

        return Task.CompletedTask;
    }
}