using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Bots.Markups;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

public interface IStateContextProxyMinimal : IAsyncDisposable
{
    /// <summary>
    /// Отправляет документы в чат
    /// </summary>
    /// <param name="documentData">Файл документа</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendDocumentAsync(FileDataBase documentData, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет документы в чат
    /// </summary>
    /// <param name="documentData">Файл документа</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendDocumentAsync(FileDataBase documentData, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет фото в чат
    /// </summary>
    /// <param name="documentData">Файл документа</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendPhotoAsync(FileDataBase documentData, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет фото в чат
    /// </summary>
    /// <param name="documentData">Файл документа</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendPhotoAsync(FileDataBase documentData, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="photoData">Файл изображения</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, FileDataBase photoData, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="photoData">Файл изображения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, FileDataBase photoData, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupMassiveList">Кнопки</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupMassiveList inlineMarkupMassiveList, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupMassiveList">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupMassiveList inlineMarkupMassiveList, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendOrUpdateTextMessageAsync(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendOrUpdateTextMessageAsync(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendTextMessageWithReplyAsync(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendTextMessageWithReplyAsync(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendTextMessageAsync(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendTextMessageAsync(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет большой текст
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendLongTextMessageAsync(string text, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет большой текст
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendLongTextMessageAsync(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет действие в чат
    /// </summary>
    /// <param name="chatAction">Действие в чат</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendChatActionAsync(ChatAction chatAction, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет основные кнопки в чате
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> RemoveMarkupAsync(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет основные кнопки в чате
    /// </summary>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> UpdateMainButtonsAsync(MainButtonMassiveList replyMarkup, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет основные кнопки в чате
    /// </summary>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> UpdateMainButtonsAsync(MainButtonMassiveList replyMarkup, string text, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет сообщение и удаляет кнопки с заменой текста
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateMarkupTextAndDropButtonAsync(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет сообщение и удаляет кнопки с заменой текста
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateMarkupTextAndDropButtonAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет сообщение с кнопкой от которого пришел запрос
    /// </summary>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task RemoveMessageAsync(int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Пересылает сообщение
    /// </summary>
    /// <param name="fromChatId">Id чата откуда берутся данные</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> ForwardMessageAsync(long fromChatId, int messageId, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Пересылает сообщение
    /// </summary>
    /// <param name="fromChatId">Id чата откуда берутся данные</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> ForwardMessageAsync(long fromChatId, int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Копирует сообщение
    /// </summary>
    /// <param name="fromChatId">Id чата откуда берутся данные</param>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="caption">Подпись/текст к изображению</param>
    /// <param name="replyToMessageId">Id сообщение на которое ответить</param>
    /// <param name="allowSendingWithoutReply">Разрешить отправку без ответа на сообщение</param>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CopyMessageAsync(
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
    /// <param name="messageId">Id сообщения</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PinChatMessageAsync(int messageId, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Фиксирует сообщение
    /// </summary>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PinChatMessageAsync(int messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Снимает фиксацию с сообщения
    /// </summary>
    /// <param name="messageId">Id сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnpinChatMessageAsync(int messageId, CancellationToken cancellationToken);

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
    Task<int> GetChatMemberCountAsync(long chatIdToCheck, CancellationToken cancellationToken);

    /// <summary>
    /// Получает информацию для пользователя по чату
    /// </summary>
    /// <param name="chatIdToCheck"></param>
    /// <param name="userIdToCheck"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatMemberData> GetChatMemberAsync(long chatIdToCheck, long userIdToCheck, CancellationToken cancellationToken);

    /// <summary>
    /// Получает информацию о списке администраторов чата
    /// </summary>
    /// <param name="chatIdToCheck"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<ChatMemberData>> GetChatAdministratorsAsync(long chatIdToCheck, CancellationToken cancellationToken);

    /// <summary>
    /// Получает информацию о чате
    /// </summary>
    /// <param name="chatIdToCheck"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TelegramChat> GetChatAsync(long chatIdToCheck, CancellationToken cancellationToken);

    /// <summary>
    /// Покидает чат
    /// </summary>
    /// <param name="chatIdToLeave"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task LeaveChatAsync(long chatIdToLeave, CancellationToken cancellationToken);
}