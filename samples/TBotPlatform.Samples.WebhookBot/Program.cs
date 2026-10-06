using System.Reflection;
using TBotPlatform.Common.Dependencies;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Samples.WebhookBot;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
var webhookUrl = Environment.GetEnvironmentVariable("TELEGRAM_WEBHOOK_URL");

if (string.IsNullOrWhiteSpace(token))
{
    Console.WriteLine("Задайте TELEGRAM_BOT_TOKEN.");
    return 1;
}

if (string.IsNullOrWhiteSpace(webhookUrl))
{
    Console.WriteLine("Задайте TELEGRAM_WEBHOOK_URL.");
    return 1;
}

var builder = WebApplication.CreateBuilder(args);
var assembly = Assembly.GetExecutingAssembly();

builder.Services
   .AddBotPlatform()
   .AddBot(new TBotSetting
   {
       BotName = "webhook",
       Token = token,
       WebhookUrl = webhookUrl,
       WebhookSecretToken = Environment.GetEnvironmentVariable("TELEGRAM_WEBHOOK_SECRET_TOKEN"),
       UpdatePolicy = new TBotSettingUpdatePolicy
       {
           Capacity = 100,
           Type =
           [
               UpdateType.Message,
               UpdateType.EditedMessage,
               UpdateType.CallbackQuery,
               UpdateType.InlineQuery,
               UpdateType.ChosenInlineResult,
               UpdateType.MyChatMember,
               UpdateType.ChatMember,
               UpdateType.ChatJoinRequest,
               UpdateType.MessageReaction,
               UpdateType.MessageReactionCount,
               UpdateType.ChatBoost,
               UpdateType.RemovedChatBoost,
               UpdateType.ChannelPost,
               UpdateType.EditedChannelPost,
               UpdateType.ShippingQuery,
               UpdateType.PreCheckoutQuery,
               UpdateType.Poll,
               UpdateType.PollAnswer,
               UpdateType.BusinessConnection,
               UpdateType.BusinessMessage,
               UpdateType.EditedBusinessMessage,
               UpdateType.DeletedBusinessMessages,
               UpdateType.PurchasedPaidMedia,
               UpdateType.ManagedBot,
               UpdateType.GuestMessage,
               UpdateType.Subscription,
               UpdateType.StoppedMessageGeneration,
           ],
       },
   })
   .AddTelegramContext()
   .AddStates(assembly)
   .AddReceivingHandler<WebhookReceivingHandler>()
   .Build()
   .AddCache()
   .AddMemoryFusionCache()
   .Build()
   .AddFactories(assembly)
   .AddHostedService()
   .Build();

var app = builder.Build();

var secretToken = Environment.GetEnvironmentVariable("TELEGRAM_WEBHOOK_SECRET_TOKEN");

app.MapPost("/api/telegram/webhook", async (HttpContext context, ITelegramUpdateProcessor processor, ILoggerFactory loggerFactory, CancellationToken ct) =>
{
    var logger = loggerFactory.CreateLogger("TelegramWebhook");

    if (!string.IsNullOrEmpty(secretToken))
    {
        var header = context.Request.Headers["X-Telegram-Bot-Api-Secret-Token"].ToString();
        if (!string.Equals(header, secretToken, StringComparison.Ordinal))
        {
            return Results.Unauthorized();
        }
    }

    // JsonBotAPI.Options is required: the Bot API uses snake_case, not the default policy.
    var update = await context.Request.ReadFromJsonAsync<Update>(JsonBotAPI.Options, ct);
    if (update is null)
    {
        return Results.BadRequest();
    }

    var result = await processor.ProcessUpdate("webhook", update, ct);
    if (result.IsSuccess)
    {
        return Results.Ok();
    }

    logger.LogError(
        "Обновление {updateId} типа {updateType} не обработано: {error}",
        update.Id,
        update.Type,
        result.Error?.Description);

    // HTTP 500 causes Telegram to retry the update; use it only for errors
    // that could be transient (state processing failure, cache unavailability, etc.).
    return Results.StatusCode(500);
});

app.Run();
return 0;
