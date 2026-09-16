using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.BackgroundServices.Base;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Extension;
using Telegram.Bot;

namespace TBotPlatform.Common.BackgroundServices;

internal class TelegramContextHostedService(
    ILogger<TelegramContextHostedService> logger,
    List<string> bots,
    IServiceProvider services,
    ITelegramUpdateProcessor updateProcessor
    ) : BackgroundServiceBase<TelegramContextHostedService>
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var taskList = bots.Select(bot => Task.Factory.StartNew(
            () => ExecuteBot(bot, stoppingToken),
            stoppingToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Current
            ).Unwrap());

        return Task.WhenAll(taskList);
    }

    private async Task ExecuteBot(string bot, CancellationToken cancellationToken)
    {
        var telegramContext = services.GetRequiredKeyedService<ITelegramContext>(bot);
        var settings = telegramContext.GetBotSetting();
        var me = await telegramContext.GetMe(cancellationToken);

        logger.LogDebug("Запущен бот {name}", me.FirstName);

        if (settings.WebhookUrl.IsNotNull())
        {
            await telegramContext.SetWebhook(
                settings.WebhookUrl!,
                secretToken: settings.WebhookSecretToken,
                cancellationToken: cancellationToken
                );

            logger.LogDebug("Webhook для бота {bot} установлен: {url}", bot, settings.WebhookUrl);

            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Ожидаемое завершение при остановке сервиса.
            }
            finally
            {
                await telegramContext.DeleteWebhook(cancellationToken: CancellationToken.None);
                logger.LogDebug("Webhook для бота {bot} удалён", bot);
            }

            return;
        }

        var offset = 0;
        var updateType = settings.UpdatePolicy.IsNotNull()
            ? settings.UpdatePolicy!.Type?.ToList()
            : null;
        var limit = settings.UpdatePolicy.IsNotNull()
            ? settings.UpdatePolicy?.Capacity
            : null;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var updates = await telegramContext.GetUpdates(offset, limit, allowedUpdates: updateType, cancellationToken: cancellationToken);
                if (updates.IsNull() || updates.Length == 0)
                {
                    await Task.Delay(settings.HostWaitMilliSecond, cancellationToken);
                    continue;
                }

                await updateProcessor.ProcessUpdates(bot, updates, cancellationToken);
                offset = updates[^1].Id + 1;

                await Task.Delay(settings.HostWaitMilliSecond, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Неожиданное исключение при обработке обновлений бота {bot}", bot);
            }
        }
    }
}
