#nullable enable
using TBotPlatform.Contracts.Statistics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface ITelegramContext
{
    /// <summary>
    /// Получает OperationGuid текущих пулов запросов к telegram
    /// </summary>
    /// <returns></returns>
    Guid GetCurrentOperation();

    /// <summary>
    /// Осуществляет прямой запрос в telegram с очередью, без логирования
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> MakeRequestAsync<T>(Func<ITelegramBotClient, Task<T>> request, CancellationToken cancellationToken);

    /// <summary>
    /// Осуществляет прямой запрос в telegram с очередью, без логирования
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request">Запрос</param>
    /// <param name="logMessage">Сообщение для логирования</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> MakeRequestAsync<T>(Func<ITelegramBotClient, Task<T>> request, TelegramContextLogMessage logMessage, CancellationToken cancellationToken);

    /// <summary>
    /// Осуществляет прямой запрос в telegram с очередью, без логирования
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task MakeRequestAsync(Func<ITelegramBotClient, Task> request, CancellationToken cancellationToken);

    /// <summary>
    /// Осуществляет прямой запрос в telegram с очередью, без логирования
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="logMessage">Сообщение для логирования</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task MakeRequestAsync(Func<ITelegramBotClient, Task> request, TelegramContextLogMessage logMessage, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет сообщение
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteMessageAsync(long chatId, int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Изменяет сообщение с inline кнопками
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> EditMessageTextAsync(
        long chatId,
        int messageId,
        string text,
        InlineKeyboardMarkup? replyMarkup,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Изменяет inline кнопки в сообщении
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> EditMessageReplyMarkupAsync(
        long chatId,
        int messageId,
        InlineKeyboardMarkup replyMarkup,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Изменяет сообщение
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> EditMessageTextAsync(long chatId, int messageId, string text, CancellationToken cancellationToken);

    Task<Message> EditMessageCaptionAsync(
        long chatId,
        int messageId,
        string caption,
        InlineKeyboardMarkup? replyMarkup,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Изменяет сообщение с изображением
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="caption">Подпись/текст к изображению</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> EditMessageCaptionAsync(long chatId, int messageId, string caption, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение с основными кнопками
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageAsync(long chatId, string text, IReplyMarkup? replyMarkup, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageAsync(long chatId, string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет действие в чат
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="chatAction">Действие в чат</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendChatActionAsync(long chatId, ChatAction chatAction, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет документ с основными кнопками
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="document">Файл документа</param>
    /// <param name="caption">Подпись/текст к документу</param>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    Task<Message> SendDocumentAsync(
        long chatId,
        InputFile document,
        string? caption,
        IReplyMarkup? replyMarkup,
        bool disableNotification,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет документ с основными кнопками
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="document">Файл документа</param>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendDocumentAsync(long chatId, InputFile document, IReplyMarkup? replyMarkup, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет документ
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="document">Файл документа</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendDocumentAsync(long chatId, InputFile document, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет изображение с основными кнопками
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="photo">Файл изображения</param>
    /// <param name="caption">Подпись/текст к изображению</param>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendPhotoAsync(
        long chatId,
        InputFile photo,
        string? caption,
        IReplyMarkup? replyMarkup,
        bool disableNotification,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет изображение
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="photo">Файл изображения</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendPhotoAsync(long chatId, InputFile photo, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Скачивает файл
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="filePath">Путь до файла</param>
    /// <param name="destination">Стрим куда запишется файл</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DownloadFileAsync(long chatId, string filePath, Stream destination, CancellationToken cancellationToken);

    /// <summary>
    /// Получает файл
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="fileId">Id документа</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<File> GetFileAsync(long chatId, string fileId, CancellationToken cancellationToken);

    /// <summary>
    /// Скачивает файл
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="fileId">Id документа</param>
    /// <param name="destination">Стрим куда запишется файл</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<File> GetInfoAndDownloadFileAsync(long chatId, string fileId, Stream destination, CancellationToken cancellationToken);

    /// <summary>
    /// Пересылает сообщение
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="fromChatId">Id чата откуда берутся данные</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> ForwardMessageAsync(long chatId, long fromChatId, int messageId, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Копирует сообщение
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="fromChatId">Id чата откуда берутся данные</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="caption">Подпись/текст к изображению</param>
    /// <param name="replyToMessageId">Id сообщение на которое ответить</param>
    /// <param name="allowSendingWithoutReply">Разрешить отправку без ответа на сообщение</param>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MessageId> CopyMessageAsync(
        long chatId,
        long fromChatId,
        int messageId,
        string caption,
        int replyToMessageId,
        bool allowSendingWithoutReply,
        IReplyMarkup replyMarkup,
        bool disableNotification,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Фиксирует сообщение
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PinChatMessageAsync(long chatId, int messageId, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Снимает фиксацию с сообщения
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnpinChatMessageAsync(long chatId, int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Снимает фиксацию всех сообщений
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnpinAllChatMessagesAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Подсчитывает число участников чата
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> GetChatMemberCountAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает информацию для пользователя по чату
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="userId">Id пользователя</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatMember> GetChatMemberAsync(long chatId, long userId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает информацию о списке администраторов чата
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatMember[]> GetChatAdministratorsAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает информацию о чате
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatFullInfo> GetChatAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Покидает чат
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task LeaveChatAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    /// Получает информацию о связи бота с бизнес-аккаунтом
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BusinessConnection> GetBusinessConnectionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет ответ клиенту на запросы типа callback
    /// </summary>
    /// <param name="callbackQueryId">Id callback на который необходимо ответить</param>
    /// <param name="text">Текст уведомления</param>
    /// <param name="showAlert">Если true, вместо уведомления в верхней части экрана чата клиент будет показывать оповещение</param>
    /// <param name="url">URL, который будет открыт клиентом пользователя. Если вы создали <see cref="Game"/> и приняли условия через <a href="https://t.me/botfather">@BotFather</a>, укажите URL, который открывает вашу игру - обратите внимание, что это сработает только в том случае, если запрос поступает от кнопки <see cref="InlineKeyboardButton"><em>CallbackGame</em></see>.
    /// В противном случае вы можете использовать ссылки типа <c>t.me/your_bot?start=XXXX</c>, которые открывают вашего бота с параметром.</param>
    /// <param name="cacheTime">Максимальное время в секундах, в течение которого результат запроса обратного вызова может отображаться на стороне клиента</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task AnswerCallbackQueryAsync(
        string callbackQueryId,
        string? text,
        bool showAlert,
        string? url,
        int? cacheTime,
        CancellationToken cancellationToken
        );
}