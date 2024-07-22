﻿using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Statistics;
using TBotPlatform.Extension;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Contexts;

internal partial class TelegramContext
{
    /// <summary>
    /// Исполнение метода в очереди с сохранением статистики
    /// </summary>
    /// <param name="method"></param>
    /// <param name="statisticMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ExecuteEnqueueSafety(Task method, StatisticMessage statisticMessage, CancellationToken cancellationToken)
    {
        try
        {
            await ExecuteEnqueueSafety(method, cancellationToken);
            await telegramStatisticContext.HandleStatisticAsync(statisticMessage, cancellationToken);
        }
        catch
        {
            await telegramStatisticContext.HandleErrorStatisticAsync(statisticMessage, cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Исполнение метода в очереди с сохранением статистики
    /// </summary>
    /// <param name="method"></param>
    /// <param name="statisticMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Message> ExecuteEnqueueSafety(Task<Message> method, StatisticMessage statisticMessage, CancellationToken cancellationToken)
    {
        try
        {
            var result = await ExecuteEnqueueSafety(method, cancellationToken);
            await telegramStatisticContext.HandleStatisticAsync(statisticMessage, cancellationToken);

            return result;
        }
        catch
        {
            await telegramStatisticContext.HandleErrorStatisticAsync(statisticMessage, cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Исполнение метода в очереди с задержкой при необходимости
    /// </summary>
    /// <param name="method"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ExecuteEnqueueSafety(Task method, CancellationToken cancellationToken)
    {
        try
        {
            await Enqueue(() => method, cancellationToken);
            return;
        }
        catch (ApiRequestException ex)
        {
            var delay = ex.Parameters.IsNotNull()
                        && ex!.Parameters!.RetryAfter.HasValue
                ? ex.Parameters.RetryAfter.Value
                : 100;

            logger.LogError(ex, "Ошибка. Задержка {delay}.", delay);

            await Task.Delay(delay, cancellationToken);
        }

        await Enqueue(() => method, cancellationToken);
    }

    /// <summary>
    /// Исполнение метода в очереди с задержкой при необходимости
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="method"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<T> ExecuteEnqueueSafety<T>(Task<T> method, CancellationToken cancellationToken)
    {
        try
        {
            return await Enqueue(() => method, cancellationToken);
        }
        catch (ApiRequestException ex)
        {
            var delay = ex.Parameters.IsNotNull()
                        && ex!.Parameters!.RetryAfter.HasValue
                ? ex.Parameters.RetryAfter.Value
                : 100;

            logger.LogError(ex, "Ошибка. Задержка {delay}.", delay);

            await Task.Delay(delay, cancellationToken);
        }

        return await Enqueue(() => method, cancellationToken);
    }
}