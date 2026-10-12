# TBotPlatform

[![NuGet version](https://img.shields.io/nuget/v/TBotPlatform.svg?style=flat-square)](https://www.nuget.org/packages/TBotPlatform/)
[![NuGet downloads](https://img.shields.io/nuget/dt/TBotPlatform.svg?style=flat-square)](https://www.nuget.org/packages/TBotPlatform/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A C# library that makes it easy to build [Telegram](https://core.telegram.org/bots/api) bots.

TBotPlatform is built on top of [Telegram.Bot](https://www.nuget.org/packages/Telegram.Bot/) and gives you additional control over your bot: a state machine, update routing, retries, rate limiting, logging, delayed-message queues, caching and distributed locks — capabilities that are hard or impossible to achieve with the raw Bot API.

## Highlights

- **State machine** — model your bot flow as states activated via attributes (`[StateActivator]`, `[StateInlineActivator]`, `[AdminStateActivator]`, ...).
- **Multi-bot** — run several bots in a single host using keyed services.
- **Resilient HTTP pipeline** — Polly retries (including `Retry-After` handling) plus request rate limiting; retries are not amplified on `429` and permanent `4xx` errors are not retried.
- **SOCKS5 proxy** — route all Telegram API traffic through a SOCKS5 proxy (RFC 1928) per bot, with optional username/password authentication, without affecting retries, rate limiting or caching.
- **Caching** — FusionCache with Redis or in-memory backends, fail-safe enabled, all extensions accept a `CancellationToken`.
- **Delayed messages** — schedule messages via a background queue.
- **Distributed locks** — built on FusionCache.
- **Result pattern** — typed, error-aware results instead of throwing exceptions everywhere.
- **Logging** — every Telegram call is traced with an operation id, request count and total elapsed time.
- **Media, inline mode and payments** — albums (`SendMediaGroup`), in-place media and keyboard updates (`EditMessageMedia`, `EditMessageReplyMarkup`) instead of delete-and-resend, inline query answers and invoices.
- **Telegram helpers** — HTML escaping and entity-aware conversion for `ParseMode.Html`, chat member checks and WebApp `initData` validation.

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

## Telegram client options

`TBotSetting` exposes the native `TelegramBotClientOptions` knobs, so you do not have to configure the client yourself:

| Property | Description |
| --- | --- |
| `BaseUrl` | Base URL of a local Bot API server. |
| `Proxy` | SOCKS5 proxy settings (`TBotSettingProxy`): host, port and optional credentials. |
| `UseTestEnvironment` | Use the Telegram test environment. |
| `RetryCount` / `RetryThreshold` | Built-in retry count and the `Retry-After` threshold of `TelegramBotClient`. |
| `RequestTimeout` | Per-request timeout of the underlying `HttpClient`. |
| `ParseMode` | Parse mode used for outgoing messages (`Html` by default). |
| `WebhookUrl` / `WebhookSecretToken` | Switches the hosted service to webhook mode. |
| `UpdatePolicy` | Allowed `UpdateType`s and update batch capacity. |

Updates are received through `TelegramBotClientExtensions.ReceiveAsync` with `ReceiverOptions`; polling errors and per-update errors are reported through `IUpdateHandler` (`TelegramUpdateHandler`) with typed `ApiRequestException` handling.

## SOCKS5 proxy

Set `TBotSetting.Proxy` to route every Telegram API request of a bot through a SOCKS5 proxy. This includes long polling, media downloads and webhook registration. The proxy is transparent to the rest of the pipeline: Polly retries, `Retry-After` handling, request rate limiting and caching behave exactly as without a proxy.

Implementation notes:

- Built on a minimal, dependency-free RFC 1928 client wired through `SocketsHttpHandler.ConnectCallback`, so the .NET `WebProxy` limitation (HTTP/CONNECT only) does not apply.
- Supports IPv4, IPv6 and domain destinations; for domains the DNS resolution is delegated to the proxy.
- Supports the no-auth and username/password (RFC 1929) authentication methods.

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
| `IStateContext` | Telegram API context: sending/editing messages, documents, the main menu, message buttons, photos, albums, inline answers and payments, with all necessary checks and validations. |
| `IStateContextMinimal` | Same as `IStateContext`, but works without a concrete state inside `IStateContextFactory`. |
| `ITelegramContext` | Context with queue and logging for Telegram: direct methods to send/edit messages, receive updates, etc. |
| `ITelegramContextLog` | Log context for `ITelegramContext`. |
| `ITelegramUpdateProcessor` | Processes single updates (webhook endpoint) or batches of updates (long polling). |
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

## Sending media, editing in place

```csharp
// Send an album (2..10 files). The file count and payloads are validated
// (MediaGroupCountException / MediaGroupDataException).
await stateContext.SendMediaGroup(files, cancellationToken: cancellationToken);

// The same album pipeline can send videos or documents.
await stateContext.SendMediaGroup(files, MediaGroupType.Video, disableNotification: true, cancellationToken);

// Explicit entity markup instead of a parse mode: tg-emoji, custom emoji,
// spoilers and quotes survive a re-send without HTML/Markdown escaping.
await stateContext.SendTextMessageWithEntities(text, message.Entities, cancellationToken);

// Public link to the message that raised the update (channels and supergroups only, otherwise null).
var link = stateContext.GetMessageLink();

// Replace the keyboard of the message that raised the callback query
// (EditMessageReplyMarkup) instead of deleting and re-sending the message.
await stateContext.UpdateInlineMarkup(markupMassiveList, cancellationToken);

// Reply with reply parameters, link preview options and a message effect.
await stateContext.SendTextMessage(
    text,
    disableNotification: false,
    replyParameters: new ReplyParameters { MessageId = messageId },
    linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true },
    messageEffectId: null,
    cancellationToken: cancellationToken);
```

When a callback message already contains a photo, `SendOrUpdateTextMessage` updates it in place through `EditMessageMedia` (caption, parse mode and keyboard are preserved) instead of deleting and re-sending it.

## Inline mode and payments

`IStateContext` also exposes `AnswerInlineQuery`, `AnswerWebAppQuery`, `AnswerPreCheckoutQuery`, `AnswerShippingQuery` and `SendInvoice`.

## Telegram and WebApp helpers

```csharp
// HTML-safe output when ParseMode is Html.
var safeText = userText.ToEscapeHtml();
var html = message.ToTelegramHtml();
var plain = html.ToPlainText();

// Chat member checks.
if (chatMember.IsAdminOrCreator() && chatMember.IsInChat()) { /* ... */ }

// Validate Telegram WebApp initData (signature + optional auth_date freshness).
if (initData.TryValidateInitData(botToken, out var fields, TimeSpan.FromHours(1)))
{
    // fields["user"], fields["auth_date"], ...
}
```

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

Several bots can share one host: call `AddBot(...)` per bot, then `AddTelegramContext()`, `AddStates(...)` and `AddReceivingHandler<T>()` for each of them. Every keyed registration (`ITelegramContext`, `IStartReceivingHandler`, state data) is isolated by `TBotSetting.BotName`, and each bot gets its own update receiver.

Use `AddRedisFusionCache(connectionString)` when the bots run in several instances: the user-state cache and distributed locks become shared, so a conversation is not split between replicas.

For webhook mode, set `TBotSetting.WebhookUrl` (and optionally `WebhookSecretToken`). The hosted service registers the webhook automatically. In your HTTP endpoint, validate the `X-Telegram-Bot-Api-Secret-Token` header, deserialize the request body to `Update` with `JsonBotAPI.Options` (snake_case naming) and pass it to `ITelegramUpdateProcessor.ProcessUpdate`. Return a non-2xx status only when you want Telegram to retry the delivery.

## Testing and CI

- `dotnet test TBotPlatform.sln` runs the NUnit test suite (`net8.0`).
- Coverage is collected with `coverlet.collector`: `dotnet test TBotPlatform.Tests/TBotPlatform.Tests.csproj --collect:"XPlat Code Coverage"`.
- GitHub Actions: `CI` builds, tests and packs on every push and pull request; `Publish to NuGet` packs and pushes on `v*` tags using the `NUGET_API_KEY` secret.


## Contacts

[Telegram](https://t.me/PBolDeveloper)
