#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Extension;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Handlers;

internal class TelegramUpdateProcessor(ILogger<TelegramUpdateProcessor> logger, IServiceProvider services) : ITelegramUpdateProcessor
{
    public Task ProcessUpdates(string botName, IReadOnlyList<Update> updates, CancellationToken cancellationToken)
    {
        if (updates.Count == 0)
        {
            return Task.CompletedTask;
        }

        var groups = updates
           .GroupBy(GetChatKey)
           .Select(group => ProcessChatUpdates(botName, [.. group], cancellationToken));

        return Task.WhenAll(groups);
    }

    public async Task<IResult> ProcessUpdate(string botName, Update update, CancellationToken cancellationToken)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        Exception? exception = null;
        long chatId = 0;

        try
        {
            MarkupNextState? markupNextState = null;

            if (update.Type == UpdateType.CallbackQuery)
            {
                var data = update.CallbackQuery?.Data;
                if (data.IsNotNull() && data!.TryParseJson<MarkupNextState>(out var newMarkupNextState))
                {
                    markupNextState = newMarkupNextState;
                }
            }

            if (!update.TryGetMessageUserData(out var telegramMessageUserData) || telegramMessageUserData.IsNull())
            {
                throw new InvalidOperationException("Не удалось обработать входящий запрос с telegram");
            }

            chatId = telegramMessageUserData!.ChatOrNull?.Id ?? 0;

            await using var scope = services.CreateAsyncScope();
            var telegramContext = scope.ServiceProvider.GetRequiredKeyedService<ITelegramContext>(botName);
            var handler = scope.ServiceProvider.GetRequiredKeyedService<IStartReceivingHandler>(botName)
                          ?? throw new InvalidOperationException("Обработчик сообщений отсутствует.");

            var resultReceivingHandler = await handler.HandleUpdate(botName, update, markupNextState, telegramMessageUserData, cancellationToken);
            return resultReceivingHandler.IsSuccess
                ? Result.Success()
                : Result.Failure(resultReceivingHandler.Error ?? ErrorResult.Failure("Не удалось обработать данные запроса с telegram"));
        }
        catch (Exception ex)
        {
            exception = ex;
            return Result.Failure(ErrorResult.Failure(ex.Message));
        }
        finally
        {
            timer.Stop();
            var logLevel = exception.IsNull() ? LogLevel.Debug : LogLevel.Error;
            logger.Log(
                logLevel,
                exception,
                "Обработка update {updateId} type {updateType} chat {chatId} за {elapsed}",
                update.Id,
                update.Type,
                chatId,
                timer.Elapsed);
        }
    }

    private async Task ProcessChatUpdates(string botName, IReadOnlyList<Update> updates, CancellationToken cancellationToken)
    {
        foreach (var update in updates)
        {
            await ProcessUpdate(botName, update, cancellationToken);
        }
    }

    private static long GetChatKey(Update update)
    {
        if (update.TryGetMessageUserData(out var data) && data?.ChatOrNull is not null)
        {
            return data.ChatOrNull.Id;
        }

        return update.Id;
    }
}
