using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.BackgroundServices.Base;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots.Config;
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
            await SetWebhookWithRetry(telegramContext, settings, bot, cancellationToken);

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

    private async Task SetWebhookWithRetry(ITelegramContext telegramContext, TBotSetting settings, string bot, CancellationToken cancellationToken)
    {
        const int maxAttempts = 3;
        var allowedUpdates = settings.UpdatePolicy?.Type;

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await telegramContext.SetWebhook(
                    settings.WebhookUrl!,
                    secretToken: settings.WebhookSecretToken,
                    allowedUpdates: allowedUpdates,
                    cancellationToken: cancellationToken
                    );

                logger.LogDebug("Webhook для бота {bot} установлен: {url}", bot, settings.WebhookUrl);
                return;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(
                    ex,
                    "Не удалось установить webhook для бота {bot} (попытка {attempt}/{maxAttempts}). Повтор через {seconds} c.",
                    bot,
                    attempt,
                    maxAttempts,
                    attempt * 2);

                await Task.Delay(TimeSpan.FromSeconds(attempt * 2), cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Не удалось установить webhook для бота {bot}", bot);
                throw;
            }
        }
    }
}
