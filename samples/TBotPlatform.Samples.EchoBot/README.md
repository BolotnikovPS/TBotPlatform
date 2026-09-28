# Echo sample

Консольный бот на TBotPlatform, демонстрирующий возможности библиотеки:

- состояние `/start` с основным меню (`StateActivator`);
- обработку кнопок основного меню (`ButtonsTypes`);
- inline-кнопки и inline-состояния (`StateInlineActivator`);
- отложенную отправку сообщения (`MakeDelayRequest`);
- постраничный вывод списка (пагинация через `GetPaginationData` / `TryParsePagination`);
- обновление только клавиатуры сообщения (`UpdateInlineMarkup`);
- собственный лог запросов в Telegram (`AddTelegramContext<TLog>`);
- режим webhook (`TBotSetting.WebhookUrl`);
- нескольких ботов в одном процессе (второй токен — переменная `TELEGRAM_BOT_TOKEN_SECOND`);
- Redis-кэш состояний и распределённые блокировки (`AddRedisFusionCache`) при заданной `TELEGRAM_REDIS_CONNECTION`.

## Состояния

| Кнопка меню | Состояние | Что показывает |
| --- | --- | --- |
| `Ping` | `PingState` | ответ `Pong` |
| `Инфо` | `InfoState` | inline-кнопки «Да» / «Нет» (`InlineYesState`, `InlineNoState`) |
| `Список` | `ListState` → `ListPageState` | 20 городов по 6 на странице; кнопки `<` и `>` приходят в `MarkupNextState.Data` с префиксом `$pag_` и разбираются `TryParsePagination`, а сообщение редактируется на месте |
| `Счётчик` | `CounterState` → `CounterClickState` | `UpdateInlineMarkup` меняет только клавиатуру (подпись «Нажатий: N»), не переотправляя сообщение |

Собственный лог подключается одной строкой: `.AddTelegramContext<EchoTelegramContextLog>()` вместо стандартного `AddTelegramContext()`. Реализация `ITelegramContextLog` получает `TelegramContextFullLogMessage` по каждому запросу, ошибки и итоговую статистику (`requestCount` / `elapsedMilliseconds`) при освобождении контекста.

## Запуск (long polling)

```bash
set TELEGRAM_BOT_TOKEN=123:abc
dotnet run --project samples/TBotPlatform.Samples.EchoBot
```

## Запуск (webhook)

```bash
set TELEGRAM_BOT_TOKEN=123:abc
set TELEGRAM_WEBHOOK_URL=https://your-domain.com/api/telegram/echo
dotnet run --project samples/TBotPlatform.Samples.EchoBot
```

В режиме webhook бот вызывает `SetWebhook`, а входящие обновления нужно передавать в `ITelegramUpdateProcessor.ProcessUpdate` из вашего HTTP-эндпоинта.

## Переменные окружения

| Переменная | Назначение |
| --- | --- |
| `TELEGRAM_BOT_TOKEN` | обязательный токен основного бота (`echo`) |
| `TELEGRAM_WEBHOOK_URL` | если задана — бот работает в режиме webhook вместо long polling |
| `TELEGRAM_BOT_TOKEN_SECOND` | если задана — платформа поднимает второго бота (`echo2`) с тем же набором состояний |
| `TELEGRAM_WEBHOOK_URL_SECOND` | webhook-адрес второго бота (по умолчанию берётся `TELEGRAM_WEBHOOK_URL`) |
| `TELEGRAM_REDIS_CONNECTION` | если задана — кэш строится на Redis (`AddRedisFusionCache` + префикс `echo` + health check), иначе используется in-memory кэш |

Redis нужен, когда приложение запущено в нескольких инстансах: кэш состояний пользователей и распределённые блокировки (`IDistributedLockFactory`) становятся общими, поэтому состояние диалога не «прыгает» между репликами.

```csharp
// Пример защиты от параллельной обработки одного и того же чата в нескольких инстансах.
public sealed class AntiFloodReceivingHandler(
    IStateFactory stateFactory,
    IStateContextFactory stateContextFactory,
    IDistributedLockFactory lockFactory
    ) : StartReceivingHandlerBase<EchoUser>(stateFactory, stateContextFactory)
{
    protected override async Task BeforeHandleAsync(string botName, Update update, EchoUser user, CancellationToken cancellationToken)
    {
        await using var distributedLock = await lockFactory.AcquireLock($"{botName}:{user.ChatId}", TimeSpan.FromSeconds(5), cancellationToken);
    }
}
```

## Расширение

`StartReceivingHandlerBase` предоставляет хуки `BeforeHandleAsync` / `AfterHandleAsync` для сквозной логики (анти-флуд, метрики, аудит). Пример простого анти-флуда:

```csharp
protected override async Task BeforeHandleAsync(string botName, Update update, EchoUser user, CancellationToken cancellationToken)
{
    // Проверяйте частоту запросов по user.ChatId и при необходимости бросайте исключение.
}
```
