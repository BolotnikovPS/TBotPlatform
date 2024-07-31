using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.StateContext;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

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
    ChatMessage ChatMessage { get; }

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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendDocumentAsync(InputFile inputFile, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="photoData">Файл изображения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, FileData photoData, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupList">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="inlineMarkupMassiveList">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessageAsync(string text, InlineMarkupMassiveList inlineMarkupMassiveList, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет или обновляет сообщение с прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessageAsync(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageWithReplyAsync(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageAsync(string text, CancellationToken cancellationToken);

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
    Task<Message> UpdateMarkupAsync(ButtonsRuleMassiveList replyMarkup, CancellationToken cancellationToken);

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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveCurrentReplyMessageAsync(CancellationToken cancellationToken);
}