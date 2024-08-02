﻿using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface ITelegramContext
{
    /// <summary>
    /// Получает OperationGuid лога текущих пулов запросов к telegram
    /// </summary>
    /// <returns></returns>
    Guid GetCurrentOperation();

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
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendTextMessageAsync(long chatId, string text, IReplyMarkup replyMarkup, bool disableNotification, CancellationToken cancellationToken);

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
    /// <param name="replyMarkup">Кнопки</param>
    /// <param name="disableNotification">Отключить уведомление пользователю</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> SendDocumentAsync(long chatId, InputFile document, IReplyMarkup replyMarkup, bool disableNotification, CancellationToken cancellationToken);

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
        string caption,
        IReplyMarkup replyMarkup,
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
    Task UnpinAllChatMessages(long chatId, CancellationToken cancellationToken);
}