# TBotPlatform

C# библиотека для облегчения создания бота [Telegram](https://core.telegram.org/bots/api)

Библиотека использует [Telegram.Bot](https://www.nuget.org/packages/Telegram.Bot/) и дает вам дополнительный контроль над вашим ботом, обновлениями, методами, логированием, очередями вызовов, которые обычно невозможно использовать с API-интерфейсом бота.

## Supported Platforms
Project targets .NET 8 at minimum.

## Термины
Термин  | Описание
------------- | -------------
Состояние  | класс выполняющий обработку данных запроса конкретного пользователя. Вызов определяется атрибутом состояния
Атрибут состояния  | атрибут определяющий поведение получения состояния, на команду от пользователя

## Интерфейсы
Термин  | Описание
------------- | -------------
IState  | методы состояния для обработки запроса
IStateFactory  | фабрика помошник, определяет состояние по атрибуту состояния
IStateBindFactory  | фабрика помошник, работает с зафиксированными состояниями
IStateContextFactory  | фабрика обработки контекста состояния, вызовы методов из IState, обновление основого (нижнего) меню пользователя
IMenuButtonFactory  | фабрика обработки основных кнопок меню
IStateContext  | конекст работы с методами telegram. Набор методов, отправка/редактирование сообщений, документов, основного (нижнего) меню, кнопок сообщений, фото и т.д., со всеми необходимыми провеками и валидациями
IStateContextMinimal  | тоже что и **IStateContext**, но для работы без конкретного состояния в **IStateContextFactory**
ITelegramContext  | контекст с механизмом очереди и логирования для работы с telegram, прямые методы по отправке/редактирование сообщений, получений обновлений и т.д.
ITelegramContextLog  | контекст логов от **ITelegramContext**
IMenuButton  | формирует кнопки основного (нижнего) меню
IStartReceivingHandler  | обрабатывает данные при поступлении запроса от telegram
ICacheService  | сервис работы с кешем
IDistributedLockFactory  | фабрика распределенной блокировка, использует **ICacheService**
IDelayQueue  | очередь для отправки сообщения с задержкой

## Атрибуты состояния
Термин  | Описание
------------- | -------------
StateActivatorBaseAttribute  | базовый атрибут включающий все переменные для определения состояния
StateActivatorAttribute  | атрибут для состояний определяющих основое (нижнее) меню
StateInlineActivatorAttribute  | атрибут для состояний определяющих кнопки в сообщениях

## DI Code
```csharp
           .AddBotPlatform()
                .AddBot(telegramSettings)
                    .AddTelegramContext()
                    .AddStates(executingAssembly)
                    .AddReceivingHandler<StartReceivingHandler>()
                    .Build()
                .AddCache()
                    .AddRedisCache(redisConnectionString)
                        .AddHealthTags(tags)
                        .AddHealthName(redisHealthName)
                    .Build()
                .Build()
                    .AddFactories(executingAssembly)
                    .AddHostedService()
                .Build()
```

### Примеры проектов
[AutoCareBot](https://t.me/mycarcarebot)
[ChatBirthdayBot](https://t.me/mychatbirthday_bot)

### Контакты
[Telegram](https://t.me/PBolDeveloper)