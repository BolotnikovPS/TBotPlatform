<h1 align="center">TBotPlatform</h1>
C# библиотека для облегчения создания бота <a href="https://core.telegram.org/bots/api">Telegram</a>

Библиотека использует <a href="https://www.nuget.org/packages/Telegram.Bot/">Telegram.Bot</a> и дает вам дополнительный контроль над вашим ботом, обновлениями, методами, логированием, очередями вызовов, которые обычно невозможно использовать с API-интерфейсом бота.

<h2>Supported Platforms</h2>
Project targets .NET 8 at minimum.

<h2>Термины</h2>
 - <b>Состояние</b> - класс выполняющий обработку данных запроса конкретного пользователя. Вызов определяется атрибутом состояния.
 <br/>- <b>Атрибут состояния</b> - атрибут определяющий поведение получения состояния, на команду от пользователя.
 <br/>- <b>Прокси</b> - механизмы взаимодействия с telegram по множеству токенов.

<h2>Интерфейсы</h2>
  - <b>IState</b> - методы состояния для обработки запроса
  <br/>- <b>IStateFactory</b> - фабрика помошник, определяет состояние по атрибуту состояния
  <br/>- <b>IStateBindFactory</b> - фабрика помошник, работает с зафиксированными состояниями
  <br/>- <b>IStateContextFactory</b> - фабрика обработки контекста состояния, вызовы методов из IState, обновление основого (нижнего) меню пользователя
  <br/>- <b>IMenuButtonFactory</b> - фабрика обработки основных кнопок меню
  <br/>- <b>IStateContext</b> - конекст работы с методами telegram. Набор методов, отправка/редактирование сообщений, документов, основного (нижнего) меню, кнопок сообщений, фото и т.д., со всеми необходимыми провеками и валидациями
  <br/>- <b>IStateContextMinimal</b> - тоже что и <b>IStateContext</b>, но для работы без конкретного состояния в <b>IStateContextFactory</b>
  <br/>- <b>ITelegramContext</b> - контекст с механизмом очереди и логирования для работы с telegram, прямые методы по отправке/редактирование сообщений, получений обновлений и т.д.
  <br/>- <b>ITelegramContextLog</b> - контекст логов от <b>ITelegramContext</b>
  <br/>- <b>IMenuButton</b> - формирует кнопки основного (нижнего) меню
  <br/>- <b>IStartReceivingHandler</b> - обрабатывает данные при поступлении запроса от telegram
  <br/>- <b>ICacheService</b> - сервис работы с кешем
  <br/>- <b>IDistributedLockFactory</b> - фабрика распределенной блокировка, использует <b>ICacheService</b>
  
<h2>Атрибут состояния</h2>
  - <b>StateActivatorBaseAttribute</b> - базовый атрибут включающий все переменные для определения состояния
  <br/>- <b>StateActivatorAttribute</b> - атрибут для состояний определяющих основое (нижнее) меню
  <br/>- <b>StateInlineActivatorAttribute</b> - атрибут для состояний определяющих кнопки в сообщениях

<h2>DI</h2>
```diff
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

<h2>Примеры проектов</h2>
  - <a href="https://t.me/mycarcarebot">AutoCareBot</a>

<h2>Контакты</h2>
  - <a href="https://t.me/PBolDeveloper">Telegram</a>
  