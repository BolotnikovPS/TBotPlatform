using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.StateContext;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface IStateContext<out T> : IAsyncDisposable
where T : UserBase
{
    /// <summary>
    /// Получает необходимость зафиксировать состояние
    /// </summary>
    /// <returns></returns>
    EBindStateType BindState { get; }

    /// <summary>
    /// Получает необходимость отправки менюшки
    /// </summary>
    /// <returns></returns>
    bool IsForceReplyLastMenu { get; }

    /// <summary>
    /// Получает месагу чата
    /// </summary>
    /// <returns></returns>
    ChatMessage ChatMessage { get; }

    /// <summary>
    /// Получает информацию о пользователе
    /// </summary>
    /// <returns></returns>
    T UserDb { get; }

    /// <summary>
    /// Получает состояние входящей кнопки инлайн меню
    /// </summary>
    /// <returns></returns>
    MarkupNextState MarkupNextState { get; }

    /// <summary>
    /// Устанавливает необходимость отправки менюшки
    /// </summary>
    void SetNeedIsForceReplyLastMenu();

    /// <summary>
    /// Устанавливает необходимость зафиксировать состояние
    /// </summary>
    /// <param name="type"></param>
    void SetBindState(EBindStateType type);

    /// <summary>
    /// Отправляет документы в чат
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendDocumentAsync(
        InputFile inputFile,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет сообщение прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text"></param>
    /// <param name="inlineMarkupList"></param>
    /// <param name="photoData"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessageAsync(
        string text,
        InlineMarkupList inlineMarkupList,
        FileData photoData,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет сообщение прикрепленными кнопками в чат
    /// </summary>
    /// <param name="text"></param>
    /// <param name="inlineMarkupList"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessageAsync(
        string text,
        InlineMarkupList inlineMarkupList,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет сообщение прикрепленными кнопками в чат
    /// Кнопки располагаются по уровню
    /// </summary>
    /// <param name="text"></param>
    /// <param name="inlineMarkupMassiveList"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessageAsync(
        string text,
        InlineMarkupMassiveList inlineMarkupMassiveList,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет сообщение прикрепленными кнопками в чат
    /// Кнопки располагаются по уровню
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendOrUpdateTextMessageAsync(
        string text,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageWithReplyAsync(
        string text,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет сообщение в чат с ответом на сообщение
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageAsync(
        string text,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправка большого текста
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendLongTextMessageAsync(
        string text,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет экшн в чат
    /// </summary>
    /// <param name="chatAction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendChatActionAsync(
        ChatAction chatAction,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет кнопки в чат
    /// </summary>
    /// <param name="replyMarkup"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> UpdateMarkupAsync(
        ButtonsRuleMassiveList replyMarkup,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Заменяет кнопки в сообщении на текст
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateMarkupTextAndDropButtonAsync(
        string text,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Заменяет кнопки в сообщении на текст
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateMarkupTextAndDropButtonAsync(
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Удаляет сообщение с кнопкой от которого пришел запрос
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveCurrentReplyMessageAsync(CancellationToken cancellationToken);
}