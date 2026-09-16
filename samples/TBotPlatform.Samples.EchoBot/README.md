# Echo sample

Консольный бот на TBotPlatform, демонстрирующий возможности библиотеки:

- состояние `/start` с основным меню (`StateActivator`);
- обработку кнопок основного меню (`ButtonsTypes`);
- inline-кнопки и inline-состояния (`StateInlineActivator`);
- отложенную отправку сообщения (`MakeDelayRequest`);
- режим webhook (`TBotSetting.WebhookUrl`).

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

## Расширение

`StartReceivingHandlerBase` предоставляет хуки `BeforeHandleAsync` / `AfterHandleAsync` для сквозной логики (анти-флуд, метрики, аудит). Пример простого анти-флуда:

```csharp
protected override async Task BeforeHandleAsync(string botName, Update update, EchoUser user, CancellationToken cancellationToken)
{
    // Проверяйте частоту запросов по user.ChatId и при необходимости бросайте исключение.
}
```
