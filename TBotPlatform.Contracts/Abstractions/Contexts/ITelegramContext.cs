using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace TBotPlatform.Contracts.Abstractions.Contexts;

public interface ITelegramContext
{
    Task<Update[]> GetUpdatesAsync(int offset, UpdateType[] allowedUpdates, CancellationToken cancellationToken);

    Task<User> GetBotInfoAsync(CancellationToken cancellationToken);

    Task DeleteMessageAsync(long chatId, int messageId, CancellationToken cancellationToken);

    Task<Message> EditMessageTextAsync(
        long chatId,
        int messageId,
        string text,
        InlineKeyboardMarkup replyMarkup,
        CancellationToken cancellationToken
        );

    Task<Message> EditMessageTextAsync(long chatId, int messageId, string text, CancellationToken cancellationToken);

    Task<Message> EditMessageCaptionAsync(
        long chatId,
        int messageId,
        string caption,
        InlineKeyboardMarkup replyMarkup,
        CancellationToken cancellationToken
        );

    Task<Message> EditMessageCaptionAsync(long chatId, int messageId, string caption, CancellationToken cancellationToken);

    Task<Message> SendTextMessageAsync(long chatId, string text, IReplyMarkup replyMarkup, CancellationToken cancellationToken);

    Task<Message> SendTextMessageAsync(long chatId, string text, CancellationToken cancellationToken);

    Task SendChatActionAsync(long chatId, ChatAction chatAction, CancellationToken cancellationToken);

    Task<Message> SendDocumentAsync(long chatId, InputFile document, IReplyMarkup replyMarkup, CancellationToken cancellationToken);

    Task<Message> SendDocumentAsync(long chatId, InputFile document, CancellationToken cancellationToken);

    Task<Message> SendPhotoAsync(
        long chatId,
        InputFile photo,
        string caption,
        IReplyMarkup replyMarkup,
        CancellationToken cancellationToken
        );

    Task<Message> SendPhotoAsync(long chatId, InputFile photo, CancellationToken cancellationToken);

    Task DownloadFileAsync(string filePath, Stream destination, CancellationToken cancellationToken);

    Task<File> GetFileAsync(string fileId, CancellationToken cancellationToken);
}