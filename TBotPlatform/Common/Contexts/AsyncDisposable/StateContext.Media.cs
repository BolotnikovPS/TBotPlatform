#nullable enable
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public Task<Message[]> SendMediaGroup(IReadOnlyList<FileDataBase> mediaDatas, CancellationToken cancellationToken)
        => SendMediaGroup(mediaDatas, MediaGroupType.Photo, disableNotification: false, cancellationToken);

    public Task<Message[]> SendMediaGroup(
        IReadOnlyList<FileDataBase> mediaDatas,
        bool disableNotification,
        CancellationToken cancellationToken
        )
        => SendMediaGroup(mediaDatas, MediaGroupType.Photo, disableNotification, cancellationToken);

    public Task<Message[]> SendMediaGroup(
        IReadOnlyList<FileDataBase> mediaDatas,
        MediaGroupType mediaGroupType,
        CancellationToken cancellationToken
        )
        => SendMediaGroup(mediaDatas, mediaGroupType, disableNotification: false, cancellationToken);

    public async Task<Message[]> SendMediaGroup(
        IReadOnlyList<FileDataBase> mediaDatas,
        MediaGroupType mediaGroupType,
        bool disableNotification,
        CancellationToken cancellationToken
        )
    {
        ChatIdValidOrThrow();
        MediaGroupValidOrThrow(mediaDatas);

        var streams = new List<Stream>(mediaDatas.Count);

        try
        {
            var media = new List<IAlbumInputMedia>(mediaDatas.Count);

            foreach (var mediaData in mediaDatas)
            {
                var stream = mgr.GetStream(mediaData.Bytes);
                streams.Add(stream);

                media.Add(CreateAlbumMedia(mediaGroupType, stream, mediaData.Name));
            }

            return await telegramContext.SendMediaGroup(
                chatId,
                media,
                disableNotification: disableNotification,
                cancellationToken: cancellationToken
                );
        }
        finally
        {
            foreach (var stream in streams)
            {
                await stream.DisposeAsync();
            }
        }
    }

    private static IAlbumInputMedia CreateAlbumMedia(MediaGroupType mediaGroupType, Stream stream, string name)
    {
        var inputFile = InputFile.FromStream(stream, name);

        return mediaGroupType switch
        {
            MediaGroupType.Video => new InputMediaVideo(inputFile),
            MediaGroupType.Document => new InputMediaDocument(inputFile),
            _ => new InputMediaPhoto(inputFile),
        };
    }

    public Task<Message> UpdateInlineMarkup(InlineMarkupMassiveList inlineMarkupMassiveList, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        CallbackQueryValidOrThrow(ChatUpdate?.CallbackQuery);

        var inlineKeyboard = inlineMarkupMassiveList.Map();

        if (inlineKeyboard.IsNull())
        {
            throw new ReplyKeyboardMarkupArgException();
        }

        return telegramContext.EditMessageReplyMarkup(
            chatId,
            ChatUpdate!.CallbackQuery!.Message!.MessageId,
            inlineKeyboard,
            cancellationToken: cancellationToken
            );
    }
}
