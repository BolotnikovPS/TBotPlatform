using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface ITelegramContext
{
    /// <summary>
    /// Получает сообщения от telegram
    /// </summary>
    /// <param name="offset">Значение последнего сообщения на единицу больше</param>
    /// <param name="allowedUpdates">Какие типы сообщений принимаем</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Update[]> GetUpdatesAsync(int offset, UpdateType[] allowedUpdates, CancellationToken cancellationToken);

    /// <summary>
    /// Получает информацию о боте
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<User> GetBotInfoAsync(CancellationToken cancellationToken);

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
        InlineKeyboardMarkup replyMarkup,
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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageAsync(long chatId, string text, IReplyMarkup replyMarkup, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет сообщение
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageAsync(long chatId, string text, CancellationToken cancellationToken);

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
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendDocumentAsync(long chatId, InputFile document, IReplyMarkup replyMarkup, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет документ
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="document">Файл документа</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendDocumentAsync(long chatId, InputFile document, CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет изображение с основными кнопками
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="photo">Файл изображения</param>
    /// <param name="caption">Подпись/текст к изображению</param>
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendPhotoAsync(
        long chatId,
        InputFile photo,
        string caption,
        IReplyMarkup replyMarkup,
        CancellationToken cancellationToken
        );

    /// <summary>
    /// Отправляет изображение
    /// </summary>
    /// <param name="chatId">Id чата</param>
    /// <param name="photo">Файл изображения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendPhotoAsync(long chatId, InputFile photo, CancellationToken cancellationToken);

    /// <summary>
    /// Скачивает файл
    /// </summary>
    /// <param name="filePath">Путь до файла</param>
    /// <param name="destination">Стрим куда запишется файл</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DownloadFileAsync(string filePath, Stream destination, CancellationToken cancellationToken);

    /// <summary>
    /// Получает файл
    /// </summary>
    /// <param name="fileId">Id документа</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<File> GetFileAsync(string fileId, CancellationToken cancellationToken);
}