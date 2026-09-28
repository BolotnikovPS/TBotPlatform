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
            // Ожидаемое завершение при остановке сервиса.
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Бот {bot} остановлен из-за необработанного исключения", bot);
        }
    }

    private async Task ExecuteBot(string bot, CancellationToken cancellationToken)
    {
        // Scoped-сервисы (включая keyed ITelegramContext) резолвятся из scope на время жизни бота,
        // иначе они захватываются из корневого провайдера (captive dependency).
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
                // Ожидаемое завершение при остановке сервиса.
            }
            finally
            {
                await telegramContext.DeleteWebhook(cancellationToken: CancellationToken.None);
                logger.LogDebug("Webhook для бота {bot} удалён", bot);
            }

            return;
        }

        // Получение обновлений делегируется Telegram.Bot (long polling с корректным offset,
        // DropPendingUpdates и типизированными ошибками) вместо ручного цикла GetUpdates.
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = settings.UpdatePolicy?.Type,

            // Telegram Bot API принимает limit только в диапазоне 1..100, иначе 400 Bad Request.
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
            // Ожидаемое завершение при остановке сервиса.
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
                // Постоянные ошибки (неверный токен, неизвестный метод): повтор не поможет.
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
