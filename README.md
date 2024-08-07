<h1 align="center">TBotPlatform</h1>
C# библиотека для создания бота <a href="https://core.telegram.org/bots/api">Telegram</a>

Библиотека использует <a href="https://www.nuget.org/packages/Telegram.Bot/">Telegram.Bot</a> и дает вам дополнительный контроль над вашим ботом, обновлениями, методами, логированием, очередями вызовов, которые обычно невозможно использовать с API-интерфейсом бота.

<h2>Supported Platforms</h2>
Project targets .NET 8 at minimum.

<h2>Термины</h2>
 - <b>Состояние</b> - класс выполняющий обработку данных запроса конкретного пользователя. Вызов определяется атрибутом состояния.
 <br/>- <b>Атрибут состояния</b> - атрибут определяющий поведение получения состояния, на команду от пользователя

 <h2>Интерфейсы</h2>
  - <b>IState</b> - методы состояния для обработки запроса
  <br/>- <b>IStateFactory</b> - помошник, помогает определить состояние по атрибуту состояния
  <br/>- <b>IStateContextFactory</b> - контекст обработка состояния, вызовы методов из IState, обновление основого (нижнего) меню пользователя
  <br/>- <b>IStateContext</b> - конекст работы с методами telegram. Набор методов, отправка/редактирование сообщений, документов, основного (нижнего) меню, кнопок сообщений, фото и т.д., со всеми необходимыми провеками и валидациями
  <br/>- <b>ITelegramContext</b> - контекст с механизмом очереди и логирования для работы с telegram, прямые методы по отправке/редактирование сообщений, получений обновлений и т.д.
  <br/>- <b>ITelegramContextLog</b> - контекст логов от ITelegramContext
  <br/>- <b>IMenuButton</b> - Получение кнопок основного (нижнего) меню

<h2>Атрибут состояния</h2>
  - <b>StateActivatorBaseAttribute</b> - базовый атрибут включающий все переменные для определения состояния
  <br/>- <b>StateActivatorAttribute</b> - атрибут для состояний определяющих основое (нижнее) меню
  <br/>- <b>StateInlineActivatorAttribute</b> - атрибут для состояний определяющих кнопки в сообщениях
