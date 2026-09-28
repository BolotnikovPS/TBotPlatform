#nullable enable
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Bots.Markups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

public interface IStateContextMinimal : IAsyncDisposable
{
    /// <summary>
    /// Получает OperationGuid текущих пулов запросов к telegram
    /// </summary>
    /// <returns></returns>
    Guid CurrentOperation { get; }

    /// <summary>
    /// Получает контекст для работы с telegram напрямую
    /// </summary>
    /// <returns></returns>
    ITelegramContext TelegramContext { get; }

    /// <summary>
    /// Отправляет документы в чат
    /// </summary>
    /// <param name="documentData">Файл документа</param>
    /// <param name="caption">Подпись/текст к документу</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendDocument(FileDataBase documentData, string? caption, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет документы в чат
    /// </summary>
    /// <param name="documentData">Файл документа</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendDocument(FileDataBase documentData, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет документы в чат
    /// </summary>
    /// <param name="documentData">Файл документа</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendDocument(FileDataBase documentData, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет фото в чат
    /// </summary>
    /// <param name="documentData">Файл документа</param>
    /// <param name="caption">Подпись/текст к изображению</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendPhoto(FileDataBase documentData, string? caption, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет фото в чат
    /// </summary>
    /// <param name="documentData">Файл документа</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendPhoto(FileDataBase documentData, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет фото в чат
    /// </summary>
    /// <param name="documentData">Файл документа</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendPhoto(FileDataBase documentData, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет альбом (группу файлов) в чат. Telegram принимает от 2 до 10 файлов в одном альбоме
    /// </summary>
    /// <param name="mediaDatas">Файлы альбома. Каждый файл отправляется как изображение</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message[]> SendMediaGroup(IReadOnlyList<FileDataBase> mediaDatas, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет альбом (группу файлов) в чат. Telegram принимает от 2 до 10 файлов в одном альбоме
    /// </summary>
    /// <param name="mediaDatas">Файлы альбома. Каждый файл отправляется как изображение</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message[]> SendMediaGroup(IReadOnlyList<FileDataBase> mediaDatas, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет альбом (группу файлов) в чат. Telegram принимает от 2 до 10 файлов в одном альбоме
    /// </summary>
    /// <param name="mediaDatas">Файлы альбома</param>
    /// <param name="mediaGroupType">Тип отправляемых файлов: фото, видео или документ</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message[]> SendMediaGroup(
        IReadOnlyList<FileDataBase> mediaDatas,
        MediaGroupType mediaGroupType,
        bool disableNotification,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет альбом (группу файлов) в чат. Telegram принимает от 2 до 10 файлов в одном альбоме
    /// </summary>
    /// <param name="mediaDatas">Файлы альбома</param>
    /// <param name="mediaGroupType">Тип отправляемых файлов: фото, видео или документ</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message[]> SendMediaGroup(
        IReadOnlyList<FileDataBase> mediaDatas,
        MediaGroupType mediaGroupType,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Обновляет только кнопки сообщения, от которого пришел запрос (без удаления и повторной отправки)
    /// </summary>
    /// <param name="inlineMarkupMassiveList">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> UpdateInlineMarkup(InlineMarkupMassiveList inlineMarkupMassiveList, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="photoData">Файл изображения</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessage(
        string text,
        InlineMarkupList inlineMarkupList,
        FileDataBase photoData,
        bool disableNotification,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="photoData">Файл изображения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupList inlineMarkupList, FileDataBase photoData, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupList inlineMarkupList, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupList inlineMarkupList, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupMassiveList">Кнопки</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupMassiveList inlineMarkupMassiveList, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupMassiveList">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupMassiveList inlineMarkupMassiveList, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessage(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessage(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageWithReply(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageWithReply(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessage(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessage(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение с расширенными параметрами: ответ на сообщение, настройка превью ссылок, эффект сообщения
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="replyParameters">Параметры ответа на сообщение</param>
    /// <param name="linkPreviewOptions">Настройки отображения превью ссылок</param>
    /// <param name="messageEffectId">Уникальный идентификатор эффекта сообщения (только для приватных чатов)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessage(
        string text,
        bool disableNotification,
        ReplyParameters? replyParameters,
        LinkPreviewOptions? linkPreviewOptions,
        string? messageEffectId,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет сообщение с явно заданными сущностями разметки (entities) вместо parse mode.
    /// Позволяет переотправить форматирование без потерь: tg-emoji, custom emoji, спойлеры, цитаты
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="entities">Сущности разметки текста</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageWithEntities(
        string text,
        IReadOnlyList<MessageEntity> entities,
        bool disableNotification,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет сообщение с явно заданными сущностями разметки (entities) вместо parse mode
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="entities">Сущности разметки текста</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageWithEntities(
        string text,
        IReadOnlyList<MessageEntity> entities,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет большой текст
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendLongTextMessage(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет большой текст
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendLongTextMessage(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет действие в чат
    /// </summary>
    /// <param name="chatAction">Действие в чат</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendChatAction(ChatAction chatAction, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет основные кнопки в чате
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> RemoveMarkup(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет основные кнопки в чате
    /// </summary>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> UpdateMainButtons(MainButtonMassiveList replyMarkup, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет основные кнопки в чате
    /// </summary>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> UpdateMainButtons(MainButtonMassiveList replyMarkup, string text, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет сообщение и удаляет кнопки с заменой текста
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateMarkupTextAndDropButton(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет сообщение и удаляет кнопки с заменой текста
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateMarkupTextAndDropButton(CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет сообщение с кнопкой от которого пришел запрос
    /// </summary>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveMessage(int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Пересылает сообщение
    /// </summary>
    /// <param name="fromChatId">Id чата откуда берутся данные</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> ForwardMessage(long fromChatId, int messageId, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Пересылает сообщение
    /// </summary>
    /// <param name="fromChatId">Id чата откуда берутся данные</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> ForwardMessage(long fromChatId, int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Копирует сообщение
    /// </summary>
    /// <param name="fromChatId">Id чата откуда берутся данные</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="caption">Подпись/текст к сообщению</param>
    /// <param name="replyToMessageId">Id сообщение на которое ответить</param>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CopyMessage(
        long fromChatId,
        int messageId,
        string caption,
        int replyToMessageId,
        ReplyMarkup replyMarkup,
        bool disableNotification,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Фиксирует сообщение
    /// </summary>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PinChatMessage(int messageId, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Фиксирует сообщение
    /// </summary>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PinChatMessage(int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Снимает фиксацию с сообщения
    /// </summary>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnpinChatMessage(int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Снимает фиксацию всех сообщений
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnpinAllChatMessages(CancellationToken cancellationToken);

    /// <summary>
    /// Подсчитывает число участников чата
    /// </summary>
    /// <param name="chatIdToCheck"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> GetChatMemberCount(long chatIdToCheck, CancellationToken cancellationToken);

    /// <summary>
    /// Получает информацию для пользователя по чату
    /// </summary>
    /// <param name="chatIdToCheck"></param>
    /// <param name="userIdToCheck"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatMember> GetChatMember(long chatIdToCheck, long userIdToCheck, CancellationToken cancellationToken);

    /// <summary>
    /// Получает информацию о списке администраторов чата
    /// </summary>
    /// <param name="chatIdToCheck"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<ChatMember>> GetChatAdministrators(long chatIdToCheck, CancellationToken cancellationToken);

    /// <summary>
    /// Получает информацию о чате
    /// </summary>
    /// <param name="chatIdToCheck"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatFullInfo> GetChat(long chatIdToCheck, CancellationToken cancellationToken);

    /// <summary>
    /// Покидает чат
    /// </summary>
    /// <param name="chatIdToLeave"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task LeaveChat(long chatIdToLeave, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет ответ клиенту на запросы типа callback
    /// </summary>
    /// <param name="text">Текст уведомления</param>
    /// <param name="showAlert">Если true, вместо уведомления в верхней части экрана чата клиент будет показывать оповещение</param>
    /// <param name="url">URL, который будет открыт клиентом пользователя. Если вы создали <c>InlineMarkupCallBackGame</c> и приняли условия через <a href="https://t.me/botfather">@BotFather</a>, укажите URL, который открывает вашу игру.
    /// В противном случае вы можете использовать ссылки типа <c>t.me/your_bot?start=XXXX</c>, которые открывают вашего бота с параметром.</param>
    /// <param name="cacheTime">Максимальное время в секундах, в течение которого результат запроса обратного вызова может отображаться на стороне клиента</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <returns></returns>
    Task AnswerCallbackQuery(
        string text,
        bool showAlert,
        string url,
        int? cacheTime,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет ответ на inline-запрос пользователя
    /// </summary>
    /// <param name="inlineQueryId">Идентификатор inline-запроса</param>
    /// <param name="results">Результаты запроса, не более 50</param>
    /// <param name="cacheTime">Время кеширования результата на стороне сервера в секундах, по умолчанию 300</param>
    /// <param name="nextOffset">Смещение для следующей страницы результатов</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AnswerInlineQuery(
        string inlineQueryId,
        IReadOnlyList<InlineQueryResult> results,
        int? cacheTime,
        string? nextOffset,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет ответ на запрос web app
    /// </summary>
    /// <param name="webAppQueryId">Идентификатор запроса из <c>WebAppQuery</c></param>
    /// <param name="result">Результат, который будет показан пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SentWebAppMessage> AnswerWebAppQuery(string webAppQueryId, InlineQueryResult result, CancellationToken cancellationToken);

    /// <summary>
    /// Подтверждает или отклоняет оплату. Ответ должен быть отправлен в течение 10 секунд
    /// </summary>
    /// <param name="preCheckoutQueryId">Идентификатор запроса</param>
    /// <param name="errorMessage">Текст ошибки, если оплата не может быть проведена. Пустое значение подтверждает оплату</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AnswerPreCheckoutQuery(string preCheckoutQueryId, string? errorMessage, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет варианты доставки или причину невозможности доставки
    /// </summary>
    /// <param name="shippingQueryId">Идентификатор запроса</param>
    /// <param name="shippingOptions">Варианты доставки, если доставка возможна</param>
    /// <param name="errorMessage">Текст ошибки, если доставка невозможна</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AnswerShippingQuery(
        string shippingQueryId,
        IReadOnlyList<ShippingOption>? shippingOptions,
        string? errorMessage,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет счет на оплату в чат
    /// </summary>
    /// <param name="title">Наименование товара, 1-32 символа</param>
    /// <param name="description">Описание товара, 1-255 символов</param>
    /// <param name="payload">Внутренний payload бота, 1-128 байт</param>
    /// <param name="currency">Трехбуквенный код валюты ISO 4217, для Telegram Stars передается <c>XTR</c></param>
    /// <param name="prices">Составляющие цены</param>
    /// <param name="providerToken">Токен платежного провайдера, пустая строка для Telegram Stars</param>
    /// <param name="providerData">Данные для платежного провайдера в формате JSON</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendInvoice(
        string title,
        string description,
        string payload,
        string currency,
        IReadOnlyList<LabeledPrice> prices,
        string? providerToken,
        string? providerData,
        CancellationToken cancellationToken
        );
}