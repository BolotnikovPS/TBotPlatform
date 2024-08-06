using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatMessages;
using TBotPlatform.Contracts.Bots.UserBases;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Handlers;

internal class TelegramUpdateHandler(ILogger<TelegramUpdateHandler> logger, ITelegramContext botClient) : ITelegramUpdateHandler
{
    public TelegramUser GetTelegramUser(Update update)
    {
        var tgUser = update.Type switch
        {
            UpdateType.CallbackQuery => update.CallbackQuery?.From,
            UpdateType.Message => update.Message?.From,
            UpdateType.MyChatMember => update.MyChatMember?.From,
            UpdateType.InlineQuery => update.InlineQuery?.From,
            UpdateType.ChosenInlineResult => update.ChosenInlineResult?.From,
            UpdateType.EditedMessage => update.EditedMessage?.From,
            UpdateType.ChannelPost => update.ChannelPost?.From,
            UpdateType.EditedChannelPost => update.EditedChannelPost?.From,
            UpdateType.ShippingQuery => update.ShippingQuery?.From,
            UpdateType.PreCheckoutQuery => update.PreCheckoutQuery?.From,
            UpdateType.ChatMember => update.ChatMember?.From,
            UpdateType.ChatJoinRequest => update.ChatJoinRequest?.From,
            _ => throw new ArgumentOutOfRangeException(),
        };

        var chat = update.Type switch
        {
            UpdateType.CallbackQuery => update.CallbackQuery?.Message?.Chat,
            UpdateType.Message => update.Message?.Chat,
            UpdateType.MyChatMember => update.MyChatMember?.Chat,
            UpdateType.EditedMessage => update.EditedMessage?.Chat,
            UpdateType.ChannelPost => update.ChannelPost?.Chat,
            UpdateType.EditedChannelPost => update.EditedChannelPost?.Chat,
            UpdateType.ChatMember => update.ChatMember?.Chat,
            UpdateType.ChatJoinRequest => update.ChatJoinRequest?.Chat,
        };

        return new(tgUser, chat);
    }

    public Task<ChatMessage> GetChatMessageAsync(TelegramUser telegramUser, Update update, CancellationToken cancellationToken)
        => GetChatMessageAsync(telegramUser.Chat.Id, update, cancellationToken);

    public async Task<ChatMessage> GetChatMessageAsync(long chatId, Update update, CancellationToken cancellationToken)
    {
        if (update.IsNull())
        {
            return default;
        }

        ChatMessageCallbackQuery callbackQueryOrNull = null;
        ChatMessageForwardFromUser forwardFromUserOrNull = null;
        ChatMessageMemberUpdate chatMessageMemberUpdate = null;

        if (update.CallbackQuery.IsNotNull()
            && update.CallbackQuery!.Message.IsNotNull()
           )
        {
            var callbackQueryMessage = update.CallbackQuery.Message!;
            var callbackQueryMessageWithImage = callbackQueryMessage.Type == MessageType.Photo;

            var callbackQueryMessageOrNull = callbackQueryMessageWithImage ? callbackQueryMessage.Caption : callbackQueryMessage.Text;

            if (callbackQueryMessageOrNull.IsNotNull())
            {
                callbackQueryOrNull = new(
                    callbackQueryMessageOrNull!,
                    callbackQueryMessageWithImage,
                    callbackQueryMessage.MessageId,
                    callbackQueryMessage.Date
                    );
            }
        }

        if (update.Message.IsNotNull()
            && (update.Message!.ForwardFromMessageId.IsNotNull()
                || update.Message!.ForwardSignature.IsNotNull()
                || update.Message!.ForwardSenderName.IsNotNull()
                || update.Message!.ForwardDate.IsNotNull()
            )
           )
        {
            UserMinimal forwardUser = null;
            UserMinimal bot = null;

            if (update.Message!.ForwardFrom.IsNotNull())
            {
                var tgUser = update.Message!.ForwardFrom;

                if (tgUser!.IsBot)
                {
                    bot = CreateUser(tgUser);
                }
                else
                {
                    forwardUser = CreateUser(tgUser);
                }
            }

            forwardFromUserOrNull = new(
                forwardUser,
                bot,
                update.Message?.ForwardFromMessageId,
                update.Message?.ForwardSignature,
                update.Message?.ForwardSenderName,
                update.Message?.ForwardDate
                );
        }

        if (update.MyChatMember.IsNotNull())
        {
            var myChatMember = update.MyChatMember;
            var chatMemberUser = CreateUser(myChatMember!.From);

            chatMessageMemberUpdate = new(
                myChatMember.Chat.IsNotNull() ? myChatMember.Chat.Id : default,
                chatMemberUser,
                myChatMember.Date,
                CreateChatMessageMember(myChatMember.OldChatMember),
                CreateChatMessageMember(myChatMember.NewChatMember)
                );
        }

        return new(
            update.Type,
            update.Message?.MessageId,
            update.Message?.Text,
            update.Message?.ReplyToMessage?.Text,
            forwardFromUserOrNull,
            callbackQueryOrNull,
            chatMessageMemberUpdate,
            await DownloadMessagePhotoAsync(chatId, update.Message, cancellationToken),
            await DownloadMessageDocumentAsync(chatId, update.Message, cancellationToken)
            );
    }

    private async Task<FileData> DownloadMessagePhotoAsync(long chatId, Message message, CancellationToken cancellationToken)
    {
        if (message.IsNotNull()
            && message.Photo.CheckAny()
           )
        {
            var photo = message.Photo![^1];
            return await DownloadFileAsync(chatId, photo.FileId, cancellationToken);
        }

        if (message.IsNull()
            || message.Document.IsNull()
            || !message.Document!.MimeType!.Contains("image")
           )
        {
            return default;
        }

        var photoDocument = message.Document;
        return await DownloadFileAsync(chatId, photoDocument.FileId, cancellationToken);
    }

    private async Task<FileData> DownloadMessageDocumentAsync(long chatId, Message message, CancellationToken cancellationToken)
    {
        if (message.IsNull()
            || message.Document.IsNull()
           )
        {
            return default;
        }

        var document = message.Document!;
        return await DownloadFileAsync(chatId, document.FileId, cancellationToken);
    }

    private async Task<FileData> DownloadFileAsync(long chatId, string fileId, CancellationToken cancellationToken)
    {
        if (chatId.IsDefault()
            || fileId.IsNull()
           )
        {
            return default;
        }

        try
        {
            var file = await botClient.GetFileAsync(chatId, fileId, cancellationToken);

            if (file.IsNull())
            {
                return default;
            }

            await using var fileStream = new MemoryStream();

            await botClient.DownloadFileAsync(chatId, file.FilePath!, fileStream, cancellationToken);

            return new()
            {
                Byte = fileStream.ToArray(),
                Name = file.FilePath,
                Size = file.FileSize!.Value,
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка скачивания {fileId}", fileId);
        }

        return default;
    }

    private static UserMinimal CreateUser(User tgUser) => new()
    {
        TgUserId = tgUser!.Id,
        UserName = tgUser.Username ?? "",
        FirstName = tgUser.FirstName,
        LastName = tgUser.LastName ?? "",
    };

    private static ChatMessageMember CreateChatMessageMember(ChatMember chatMember)
        => new(chatMember.Status);
}