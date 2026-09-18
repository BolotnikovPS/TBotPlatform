# Webhook sample

Minimal API ASP.NET Core бот на TBotPlatform с приёмом обновлений через Telegram Webhook.

## Запуск

```bash
set TELEGRAM_BOT_TOKEN=123:abc
set TELEGRAM_WEBHOOK_URL=https://your-domain.com/api/telegram/webhook
dotnet run --project samples/TBotPlatform.Samples.WebhookBot
```

По желанию задайте секрет:

```bash
set TELEGRAM_WEBHOOK_SECRET_TOKEN=super-secret
```

## Эндпоинт

`POST /api/telegram/webhook`

Библиотека сама вызывает `SetWebhook` при старте. В эндпоинте:

- проверяется заголовок `X-Telegram-Bot-Api-Secret-Token` (если задан);
- тело десериализуется в `Update` и передаётся в `ITelegramUpdateProcessor.ProcessUpdate`;
- при ошибке обработки возвращается HTTP 500, чтобы Telegram повторил доставку update.
