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

- проверяется заголовок `X-Telegram-Bot-Api-Secret-Token` (если задан) — при несовпадении возвращается HTTP 401, и доставка не повторяется;
- тело десериализуется в `Update` через `JsonBotAPI.Options` (snake_case-имена Bot API) и передаётся в `ITelegramUpdateProcessor.ProcessUpdate`;
- ошибки обработки логируются (update id, тип, текст ошибки) и возвращается HTTP 500 — только в этом случае Telegram повторит доставку update.
