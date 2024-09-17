<h1 align="center">TBotPlatform</h1>
C# библиотека для облегчения создания бота <a href="https://core.telegram.org/bots/api">Telegram</a>

Библиотека использует <a href="https://www.nuget.org/packages/Telegram.Bot/">Telegram.Bot</a> и дает вам дополнительный контроль над вашим ботом, обновлениями, методами, логированием, очередями вызовов, которые обычно невозможно использовать с API-интерфейсом бота.

<h2>Supported Platforms</h2>
Project targets .NET 8 at minimum.

<h2>Термины</h2>
 - <b>Состояние</b> - класс выполняющий обработку данных запроса конкретного пользователя. Вызов определяется атрибутом состояния.
 <br/>- <b>Атрибут состояния</b> - атрибут определяющий поведение получения состояния, на команду от пользователя

 <h2>Интерфейсы</h2>
  - <b>IState</b> - методы состояния для обработки запроса
  <br/>- <b>IStateFactory</b> - помошник, помогает определить состояние по атрибуту состояния
  <br/>- <b>IStateBind</b> - помошник, помогает работать с зафиксированными состояниями
  <br/>- <b>IStateContextFactory</b> - контекст обработка состояния, вызовы методов из IState, обновление основого (нижнего) меню пользователя
  <br/>- <b>IMenuButtonFactory</b> - Обновлениие основных кнопок меню
  <br/>- <b>IDistributedLockFactory</b> - Распределенная блокировка
  <br/>- <b>IStateContext</b> - конекст работы с методами telegram. Набор методов, отправка/редактирование сообщений, документов, основного (нижнего) меню, кнопок сообщений, фото и т.д., со всеми необходимыми провеками и валидациями
  <br/>- <b>IStateContextMinimal</b> - тоже что и <b>IStateContext</b>, но для работы без конкретного состояния в <b>IStateContextFactory</b>
  <br/>- <b>ITelegramContext</b> - контекст с механизмом очереди и логирования для работы с telegram, прямые методы по отправке/редактирование сообщений, получений обновлений и т.д.
  <br/>- <b>ITelegramContextLog</b> - контекст логов от ITelegramContext
  <br/>- <b>IMenuButton</b> - Получение кнопок основного (нижнего) меню
  <br/>- <b>IStartReceivingHandler</b> - Обрабатывает данные при поступлении запроса от telegram
  <br/>- <b>ITelegramMappingHandler</b> - Маппирует запрос от telegram в формат данных проекта
  <br/>- <b>ITelegramUpdateHandler</b> - Маппирует данные от telegram в формат данных проекта

<h2>Атрибут состояния</h2>
  - <b>StateActivatorBaseAttribute</b> - базовый атрибут включающий все переменные для определения состояния
  <br/>- <b>StateActivatorAttribute</b> - атрибут для состояний определяющих основое (нижнее) меню
  <br/>- <b>StateInlineActivatorAttribute</b> - атрибут для состояний определяющих кнопки в сообщениях

<h2>DI</h2>
  - <b>AddTelegramContext</b> - зависимости для работы с telegram. Включает инфтерфейсы: <b/>ITelegramContextLog</b>, <b/>ITelegramContext</b>, <b/>ITelegramUpdateHandler</b>, <b/>ITelegramMappingHandler</b>
  <br/>- <b>AddTelegramClientHostedService</b> - включает все зависимости <b>AddTelegramContext</b> и добавляет HostedService для обработки обновлений (не webhook)
  <br/>- <b>AddReceivingHandler</b> - добавляет зависимость для интерфейса <b>IStartReceivingHandler</b>
  <br/>- <b>AddCache</b> - добавляение кеша для работы функционала состоний. Базовый кеш - Redis.
  <br/>- <b>AddFactories</b> - добавляение интерфейса фабрик: <b>IStateFactory</b>, <b>IStateBind</b>, <b>IStateContextFactory</b>, <b>IMenuButtonFactory</b>
  <br/>- <b>AddStates</b> - добавляет все доступные состояния по интерфейсу <b>IState</b>
