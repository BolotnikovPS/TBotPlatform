using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Common.BackgroundServices.Base;
using TBotPlatform.Common.Handlers;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;

namespace TBotPlatform.Common.BackgroundServices;

internal class TelegramContextHostedService(
    ILogger<TelegramContextHostedService> logger,
    List<string> bots,
    IServiceProvider services,
    ITelegramUpdateProcessor updateProcessor
    ) : BackgroundServiceBase<TelegramContextHostedService>
{
    private const int UpdatesLimitMin = 1;
    private const int UpdatesLimitMax = 100;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var taskList = bots.Select(bot => Task.Factory.StartNew(
            () => ExecuteBotSafe(bot, stoppingToken),
            stoppingToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default
            ).Unwrap());

        return Task.WhenAll(taskList);
    }

    private async Task ExecuteBotSafe(string bot, CancellationToken cancellationToken)
    {
        try
        {
            await ExecuteBot(bot, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Expected termination due to service shutdown.
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Бот {bot} остановлен из-за необработанного исключения", bot);
        }
    }

    private async Task ExecuteBot(string bot, CancellationToken cancellationToken)
    {
        // Scoped services (including keyed ITelegramContext) are resolved from the scope for the bot's lifetime,
        // otherwise they are captured from the root provider (captive dependency).
        await using var botScope = services.CreateAsyncScope();

        var telegramContext = botScope.ServiceProvider.GetRequiredKeyedService<ITelegramContext>(bot);
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
                // Expected termination due to service shutdown.
            }
            finally
            {
                await telegramContext.DeleteWebhook(cancellationToken: CancellationToken.None);
                logger.LogDebug("Webhook для бота {bot} удалён", bot);
            }

            return;
        }

        // Updates retrieval is delegated to Telegram.Bot (long polling with correct offset,
        // DropPendingUpdates and typed errors) instead of manual GetUpdates loop.
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = settings.UpdatePolicy?.Type,

            // Telegram Bot API accepts limit only in the range 1..100, otherwise 400 Bad Request.
            Limit = settings.UpdatePolicy?.Capacity is { } capacity
                ? Math.Clamp(capacity, UpdatesLimitMin, UpdatesLimitMax)
                : null,
        };

        var updateHandler = new TelegramUpdateHandler(updateProcessor, bot, logger);

        try
        {
            await telegramContext.ReceiveAsync(updateHandler, receiverOptions, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Expected termination due to service shutdown.
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
            catch (ApiRequestException ex) when (ex.ErrorCode is 401 or 404)
            {
                // Permanent errors (invalid token, unknown method): retrying will not help.
                logger.LogError(
                    ex,
                    "Не удалось установить webhook для бота {bot}: Telegram API вернул {errorCode}",
                    bot,
                    ex.ErrorCode
                    );

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
