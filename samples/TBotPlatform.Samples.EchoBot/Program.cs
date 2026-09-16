using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using TBotPlatform.Common.Dependencies;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Samples.EchoBot;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
var webhookUrl = Environment.GetEnvironmentVariable("TELEGRAM_WEBHOOK_URL");

if (string.IsNullOrWhiteSpace(token))
{
    Console.WriteLine("Задайте TELEGRAM_BOT_TOKEN.");
    return 1;
}

var builder = Host.CreateApplicationBuilder(args);
var assembly = Assembly.GetExecutingAssembly();

builder.Services
   .AddBotPlatform()
   .AddBot(new TBotSetting
   {
       BotName = "echo",
       Token = token,
       WebhookUrl = string.IsNullOrWhiteSpace(webhookUrl) ? null : webhookUrl,
   })
   .AddTelegramContext()
   .AddStates(assembly)
   .AddReceivingHandler<EchoReceivingHandler>()
   .Build()
   .AddCache()
   .AddMemoryFusionCache()
   .Build()
   .AddFactories(assembly)
   .AddHostedService()
   .Build();

var host = builder.Build();
await host.RunAsync();
return 0;
