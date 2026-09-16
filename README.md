# TBotPlatform

[![NuGet version](https://img.shields.io/nuget/v/TBotPlatform.svg?style=flat-square)](https://www.nuget.org/packages/TBotPlatform/)
[![NuGet downloads](https://img.shields.io/nuget/dt/TBotPlatform.svg?style=flat-square)](https://www.nuget.org/packages/TBotPlatform/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A C# library that makes it easy to build [Telegram](https://core.telegram.org/bots/api) bots.

TBotPlatform is built on top of [Telegram.Bot](https://www.nuget.org/packages/Telegram.Bot/) and gives you additional control over your bot: a state machine, update routing, retries, rate limiting, logging, delayed-message queues, caching and distributed locks — capabilities that are hard or impossible to achieve with the raw Bot API.

## Highlights

- **State machine** — model your bot flow as states activated via attributes (`[StateActivator]`, `[StateInlineActivator]`, `[AdminStateActivator]`, ...).
- **Multi-bot** — run several bots in a single host using keyed services.
- **Resilient HTTP pipeline** — Polly retries (including `Retry-After` handling) plus request rate limiting.
- **Caching** — FusionCache with Redis or in-memory backends, fail-safe enabled.
- **Delayed messages** — schedule messages via a background queue.
- **Distributed locks** — built on FusionCache.
- **Result pattern** — typed, error-aware results instead of throwing exceptions everywhere.
- **Logging** — every Telegram call is traced with an operation id.

## Supported Platforms

Multi-targets .NET 8, .NET 9 and .NET 10.

## Quick Start

1. Install the package:

   ```bash
   dotnet add package TBotPlatform
   ```

2. Define a user, states and a menu, then wire everything up:

   ```csharp
   builder.Services
       .AddBotPlatform()
           .AddBot(new TBotSetting { BotName = "echo", Token = token })
               .AddTelegramContext()
               .AddStates(Assembly.GetExecutingAssembly())
               .AddReceivingHandler<EchoReceivingHandler>()
               .Build()
           .AddCache()
               .AddMemoryFusionCache()
               .Build()
           .AddFactories(Assembly.GetExecutingAssembly())
           .AddHostedService()
       .Build();
   ```

See the [samples](samples/TBotPlatform.Samples.EchoBot) folder for a complete runnable echo bot.

## Concepts

| Term | Description |
| --- | --- |
| State | A class that handles a request from a specific user. Selected by a state attribute. |
| State attribute | An attribute that maps a user command to a state. |

## Interfaces

| Interface | Description |
| --- | --- |
| `IState` | State methods for handling a request. |
| `IStateFactory` | Helper factory; resolves a state by its attribute. |
| `IStateBindFactory` | Helper factory; works with bound states. |
| `IStateContextFactory` | Creates state contexts, invokes `IState` methods and refreshes the main (bottom) menu. |
| `IMenuButtonFactory` | Builds the main menu buttons. |
| `IStateContext` | Telegram API context: sending/editing messages, documents, the main menu, message buttons, photos, etc., with all necessary checks and validations. |
| `IStateContextMinimal` | Same as `IStateContext`, but works without a concrete state inside `IStateContextFactory`. |
| `ITelegramContext` | Context with queue and logging for Telegram: direct methods to send/edit messages, receive updates, etc. |
| `ITelegramContextLog` | Log context for `ITelegramContext`. |
| `IMenuButton` | Builds the main (bottom) menu buttons. |
| `IStartReceivingHandler` | Handles incoming Telegram requests. |
| `IKeyInCache` | A cache item with a key. |
| `IDistributedLockFactory` | Distributed lock factory, based on FusionCache. |
| `IDelayQueue` | Queue for sending delayed messages. |

## State attributes

| Attribute | Description |
| --- | --- |
| `StateActivatorBaseAttribute` | Base attribute holding all state-defining properties. |
| `StateActivatorAttribute` | For states that define the main (bottom) menu. |
| `StateInlineActivatorAttribute` | For states that define inline message buttons. |
| `AdminStateActivatorAttribute` | Admin variant of `StateActivatorAttribute`. |
| `AdminStateInlineActivatorAttribute` | Admin variant of `StateInlineActivatorAttribute`. |

## DI configuration

```csharp
services
    .AddBotPlatform()
        .AddBot(telegramSettings)
            .AddTelegramContext()
            .AddStates(executingAssembly)
            .AddReceivingHandler<StartReceivingHandler>()
            .Build()
        .AddCache()
            .AddRedisFusionCache(redisConnectionString)
                .AddHealthTags(tags)
                .AddHealthName(redisHealthName)
            .Build()
        .Build()
        .AddFactories(executingAssembly)
        .AddHostedService()
    .Build();
```

For webhook mode, set `TBotSetting.WebhookUrl` (and optionally `WebhookSecretToken`). The hosted service registers the webhook automatically. In your HTTP endpoint, deserialize the request body to `Update` and pass it to `ITelegramUpdateProcessor.ProcessUpdate`.

## Examples

- [AutoCareBot](https://t.me/mycarcarebot)
- [ChatBirthdayBot](https://t.me/mychatbirthday_bot)

## Contacts

[Telegram](https://t.me/PBolDeveloper)
