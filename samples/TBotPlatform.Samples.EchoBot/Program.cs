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

// Мультибот: если задан токен второго бота, платформа поднимает для него отдельный получатель обновлений.
// Все keyed-регистрации (ITelegramContext, IStartReceivingHandler, StateFactoryDataCollection) изолированы по BotName,
// состояния и обработчик переиспользуются.
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

// Redis нужен, когда ботов обслуживает несколько инстансов приложения:
// кэш состояний и распределённые блокировки становятся общими.
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
