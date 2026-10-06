using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using TBotPlatform.Common.Dependencies;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Samples.EchoBot;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
var webhookUrl = Environment.GetEnvironmentVariable("TELEGRAM_WEBHOOK_URL");
var secondToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN_SECOND");
var secondWebhookUrl = Environment.GetEnvironmentVariable("TELEGRAM_WEBHOOK_URL_SECOND");
var redisConnection = Environment.GetEnvironmentVariable("TELEGRAM_REDIS_CONNECTION");

if (string.IsNullOrWhiteSpace(token))
{
    Console.WriteLine("Задайте TELEGRAM_BOT_TOKEN.");
    return 1;
}

var builder = Host.CreateApplicationBuilder(args);
var assembly = Assembly.GetExecutingAssembly();

string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

var platform = builder.Services
   .AddBotPlatform()
   .AddBot(new TBotSetting
   {
       BotName = "echo",
       Token = token,
       WebhookUrl = Normalize(webhookUrl),
   })
   .AddTelegramContext<EchoTelegramContextLog>()
   .AddStates(assembly)
   .AddReceivingHandler<EchoReceivingHandler>()
   .Build();

// Multi-bot: if a second bot token is set, the platform starts a separate update receiver for it.
// All keyed registrations (ITelegramContext, IStartReceivingHandler, StateFactoryDataCollection) are isolated by BotName,
// states and the handler are reused.
if (!string.IsNullOrWhiteSpace(secondToken))
{
    Console.WriteLine("Поднимаю второго бота: echo2.");

    platform = platform
       .AddBot(new TBotSetting
       {
           BotName = "echo2",
           Token = secondToken,
           WebhookUrl = Normalize(secondWebhookUrl ?? webhookUrl),
       })
       .AddTelegramContext<EchoTelegramContextLog>()
       .AddStates(assembly)
       .AddReceivingHandler<EchoReceivingHandler>()
       .Build();
}

var cacheBuilder = platform.AddCache();

// Redis is needed when several application instances serve the bots:
// the state cache and distributed locks become shared.
if (!string.IsNullOrWhiteSpace(redisConnection))
{
    Console.WriteLine("Использую Redis-кэш.");

    cacheBuilder
       .AddRedisFusionCache(redisConnection)
       .AddPrefix("echo")
       .AddHealthName("redis")
       .AddHealthTags(new[] { "cache", "redis" })
       .Build();
}
else
{
    Console.WriteLine("Использую in-memory кэш (TELEGRAM_REDIS_CONNECTION не задан).");

    cacheBuilder.AddMemoryFusionCache();
}

cacheBuilder
   .Build()
   .AddFactories(assembly)
   .AddHostedService()
   .Build();

var host = builder.Build();
await host.RunAsync();
return 0;
