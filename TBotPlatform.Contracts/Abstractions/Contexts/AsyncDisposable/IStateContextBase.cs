#nullable enable
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.Markups.InlineMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

public interface IStateContextBase
{
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
        IReplyMarkup replyMarkup,
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
    /// <param name="url">URL, который будет открыт клиентом пользователя. Если вы создали <see cref="InlineMarkupCallBackGame"/> и приняли условия через <a href="https://t.me/botfather">@BotFather</a>, укажите URL, который открывает вашу игру.
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
}