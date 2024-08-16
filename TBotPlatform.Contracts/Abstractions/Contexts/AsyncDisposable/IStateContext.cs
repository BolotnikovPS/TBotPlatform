using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.StateContext;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

public interface IStateContext : IAsyncDisposable
{
    /// <summary>
    /// Признак необходимости фиксации состояния
    /// </summary>
    /// <returns></returns>
    EBindStateType BindState { get; }

    /// <summary>
    /// Признак необходимости отправки меню
    /// </summary>
    /// <returns></returns>
    bool IsForceReplyLastMenu { get; }

    /// <summary>
    /// Информация о сообщении из чата
    /// </summary>
    /// <returns></returns>
    ChatUpdate ChatUpdate { get; }

    /// <summary>
    /// Информация о состоянии входящей кнопки inline меню
    /// </summary>
    /// <returns></returns>
    MarkupNextState MarkupNextState { get; }

    /// <summary>
    /// Получает OperationGuid лога текущих пулов запросов к telegram
    /// </summary>
    /// <returns></returns>
    Guid GetCurrentOperation();

    /// <summary>
    /// Устанавливает необходимость отправки меню
    /// </summary>
    void SetNeedIsForceReplyLastMenu();

    /// <summary>
    /// Устанавливает необходимость зафиксировать состояние
    /// </summary>
    /// <param name="type">Тип биндинга</param>
    void SetBindState(EBindStateType type);

    /// <summary>
    /// Отправляет документы в чат
    /// </summary>
    /// <param name="inputFile">Файл документа</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendDocumentAsync(InputFile inputFile, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет документы в чат
    /// </summary>
    /// <param name="inputFile">Файл документа</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendDocumentAsync(InputFile inputFile, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="photoData">Файл изображения</param>
    /// <param name="disableNotification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, FileData photoData, bool disableNotification, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="photoData">Файл изображения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, FileData photoData, CancellationToken cancellationToken);

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
    /// Обновляет кнопки в чате
    /// </summary>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatResult> UpdateMarkupAsync(ButtonsRuleMassiveList replyMarkup, CancellationToken cancellationToken);

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
}