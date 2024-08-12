using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using TBotPlatform.Contracts.Bots.Locations;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Handlers;

internal class TelegramUpdateHandler(ILogger<TelegramUpdateHandler> logger, ITelegramContext botClient) : ITelegramUpdateHandler
{
    public TelegramMessageUserData GetTelegramMessageUserData(Update update)
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
            _ => null,
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
            _ => null,
        };

        return new(CreateUser(tgUser), CreateChat(chat));
    }

    public Task<ChatUpdate> GetChatUpdateAsync(TelegramMessageUserData telegramUser, Update update, CancellationToken cancellationToken)
        => GetChatMessageAsync(telegramUser?.ChatOrNull?.Id ?? 0, update, cancellationToken);

    public async Task<ChatUpdate> GetChatMessageAsync(long chatId, Update update, CancellationToken cancellationToken)
    {
        if (update.IsNull())
        {
            return new(EChatUpdateType.None, new(null!, null, null, null, null, null));
        }

        var message = new ChatUpdateMessage(null!, null, null, null, null, null);
        ChatUpdateMessage editMessageOrNull = null;
        ChatUpdateMessage channelPostOrNull = null;
        ChatUpdateMessage editChannelPostOrNull = null;
        ChatUpdateCallbackQuery callbackQueryOrNull = null;
        ChatUpdateMemberUpdate myMemberUpdateOrNull = null;
        ChatUpdateMemberUpdate memberUpdateOrNull = null;
        ChatUpdateJoinRequest joinRequestOrNull = null;
        ChatUpdateInlineQuery inlineQueryOrNull = null;
        ChatUpdateChosenInlineResult chosenInlineResultOrNull = null;
        ChatUpdatePoll chatUpdatePollOrNull = null;
        ChatUpdatePollAnswer pollAnswerOrNull = null;
        ChatUpdateShippingQuery shippingQueryOrNull = null;
        ChatUpdatePreCheckoutQuery preCheckoutQueryOrNull = null;

        switch (update.Type)
        {
            case UpdateType.Message:
                {
                    if (update.Message.IsNotNull())
                    {
                        message = await CreateChatUpdateMessageAsync(chatId, update.Message, cancellationToken);
                    }
                }
                break;

            case UpdateType.EditedMessage:
                {
                    if (update.EditedMessage.IsNotNull())
                    {
                        editMessageOrNull = await CreateChatUpdateMessageAsync(chatId, update.EditedMessage, cancellationToken);
                    }
                }
                break;

            case UpdateType.ChannelPost:
                {
                    if (update.ChannelPost.IsNotNull())
                    {
                        channelPostOrNull = await CreateChatUpdateMessageAsync(chatId, update.ChannelPost, cancellationToken);
                    }
                }
                break;

            case UpdateType.EditedChannelPost:
                {
                    if (update.EditedChannelPost.IsNotNull())
                    {
                        editChannelPostOrNull = await CreateChatUpdateMessageAsync(chatId, update.EditedChannelPost, cancellationToken);
                    }
                }
                break;

            case UpdateType.CallbackQuery:
                {
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
                }
                break;

            case UpdateType.ChatMember:
                {
                    if (update.ChatMember.IsNotNull())
                    {
                        memberUpdateOrNull = CreateChatMember(update.ChatMember);
                    }
                }
                break;

            case UpdateType.MyChatMember:
                {
                    if (update.MyChatMember.IsNotNull())
                    {
                        myMemberUpdateOrNull = CreateChatMember(update.MyChatMember);
                    }
                }
                break;

            case UpdateType.ChatJoinRequest:
                {
                    if (update.ChatJoinRequest.IsNotNull())
                    {
                        var chatJoinRequest = update.ChatJoinRequest;

                        joinRequestOrNull = new(
                            CreateChat(chatJoinRequest?.Chat),
                            CreateUser(chatJoinRequest?.From),
                            chatJoinRequest!.UserChatId,
                            chatJoinRequest!.Date
                            );
                    }
                }
                break;

            case UpdateType.InlineQuery:
                {
                    if (update.InlineQuery.IsNotNull())
                    {
                        var inlineQuery = update.InlineQuery;

                        inlineQueryOrNull = new(
                            inlineQuery!.Id,
                            CreateUser(inlineQuery.From),
                            inlineQuery.Query,
                            inlineQuery.Offset,
                            inlineQuery.ChatType
                            );
                    }
                }
                break;

            case UpdateType.ChosenInlineResult:
                {
                    if (update.ChosenInlineResult.IsNotNull())
                    {
                        var inlineQuery = update.ChosenInlineResult;

                        chosenInlineResultOrNull = new(
                            inlineQuery!.ResultId,
                            CreateUser(inlineQuery.From),
                            inlineQuery.InlineMessageId,
                            inlineQuery.Query
                            );
                    }
                }
                break;

            case UpdateType.Poll:
                {
                    if (update.Poll.IsNotNull())
                    {
                        var pool = update.Poll;
                        ChatMessagePollOption[] optionsOrNull = null;

                        if (pool!.Options.IsNotNull())
                        {
                            optionsOrNull = pool
                                           .Options
                                           .Select(z => new ChatMessagePollOption(z.Text, z.VoterCount))
                                           .ToArray();
                        }

                        CharMessageEntity[] explanationEntitiesOrNull = null;

                        if (pool.ExplanationEntities.IsNotNull())
                        {
                            explanationEntitiesOrNull = pool.ExplanationEntities!
                                                            .Select(
                                                                 z => new CharMessageEntity(
                                                                     z.Type,
                                                                     z.Offset,
                                                                     z.Length,
                                                                     z.Url,
                                                                     CreateUser(z.User)
                                                                     )
                                                                 )
                                                            .ToArray();
                        }

                        chatUpdatePollOrNull = new(
                            pool.Id,
                            pool.Question,
                            optionsOrNull,
                            pool.TotalVoterCount,
                            pool.IsClosed,
                            pool.IsAnonymous,
                            pool.Type,
                            pool.AllowsMultipleAnswers,
                            pool.CorrectOptionId,
                            pool.Explanation,
                            explanationEntitiesOrNull,
                            pool.OpenPeriod,
                            pool.CloseDate
                            );
                    }
                }
                break;

            case UpdateType.PollAnswer:
                {
                    if (update.PollAnswer.IsNotNull())
                    {
                        var poolAnswer = update.PollAnswer;

                        pollAnswerOrNull = new(poolAnswer!.PollId, CreateUser(poolAnswer.User), poolAnswer.OptionIds);
                    }
                }
                break;

            case UpdateType.ShippingQuery:
                {
                    if (update.ShippingQuery.IsNotNull())
                    {
                        var shippingQuery = update.ShippingQuery;
                        ChatMessageShippingAddress shippingAddressOrNull = null;

                        if (shippingQuery!.ShippingAddress.IsNotNull())
                        {
                            shippingAddressOrNull = new(
                                shippingQuery!.ShippingAddress.CountryCode,
                                shippingQuery.ShippingAddress.State,
                                shippingQuery.ShippingAddress.City,
                                shippingQuery.ShippingAddress.StreetLine1,
                                shippingQuery.ShippingAddress.StreetLine2,
                                shippingQuery.ShippingAddress.PostCode
                                );
                        }

                        shippingQueryOrNull = new(shippingQuery.Id, CreateUser(shippingQuery.From), shippingQuery.InvoicePayload, shippingAddressOrNull);
                    }
                }
                break;

            case UpdateType.PreCheckoutQuery:
                {
                    if (update.PreCheckoutQuery.IsNotNull())
                    {
                        var pre = update.PreCheckoutQuery;
                        ChatMessageOrderInfo orderInfoOrNull = null;

                        if (pre!.OrderInfo.IsNotNull())
                        {
                            var orderInfo = pre!.OrderInfo;
                            ChatMessageShippingAddress shippingAddressOrNull = null;

                            if (orderInfo!.ShippingAddress.IsNotNull())
                            {
                                shippingAddressOrNull = new(
                                    orderInfo!.ShippingAddress!.CountryCode,
                                    orderInfo.ShippingAddress.State,
                                    orderInfo.ShippingAddress.City,
                                    orderInfo.ShippingAddress.StreetLine1,
                                    orderInfo.ShippingAddress.StreetLine2,
                                    orderInfo.ShippingAddress.PostCode
                                    );
                            }

                            orderInfoOrNull = new(orderInfo!.Name, orderInfo.PhoneNumber, orderInfo.Email, shippingAddressOrNull);
                        }

                        preCheckoutQueryOrNull = new(
                            pre.Id,
                            CreateUser(pre.From),
                            pre.Currency,
                            pre.TotalAmount,
                            pre.InvoicePayload,
                            pre.ShippingOptionId,
                            orderInfoOrNull
                            );
                    }
                }
                break;

            case UpdateType.Unknown:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        var chatUpdateType = update.Type switch
        {
            UpdateType.Message => GetMessageUpdateType(),
            UpdateType.EditedMessage => EChatUpdateType.EditedMessage,
            UpdateType.InlineQuery => EChatUpdateType.InlineQuery,
            UpdateType.ChosenInlineResult => EChatUpdateType.ChosenInlineResult,
            UpdateType.CallbackQuery => EChatUpdateType.CallbackQuery,
            UpdateType.ChannelPost => EChatUpdateType.ChannelPost,
            UpdateType.EditedChannelPost => EChatUpdateType.EditedChannelPost,
            UpdateType.ShippingQuery => EChatUpdateType.ShippingQuery,
            UpdateType.PreCheckoutQuery => EChatUpdateType.PreCheckoutQuery,
            UpdateType.Poll => EChatUpdateType.Poll,
            UpdateType.PollAnswer => EChatUpdateType.PollAnswer,
            UpdateType.MyChatMember => EChatUpdateType.MyChatMember,
            UpdateType.ChatMember => EChatUpdateType.ChatMember,
            UpdateType.ChatJoinRequest => EChatUpdateType.ChatJoinRequest,
            _ => EChatUpdateType.None,
        };

        return new(
            chatUpdateType,
            message,
            editMessageOrNull,
            channelPostOrNull,
            editChannelPostOrNull,
            callbackQueryOrNull,
            myMemberUpdateOrNull,
            memberUpdateOrNull,
            joinRequestOrNull,
            inlineQueryOrNull,
            chosenInlineResultOrNull,
            chatUpdatePollOrNull,
            pollAnswerOrNull,
            shippingQueryOrNull,
            preCheckoutQueryOrNull
            );

        EChatUpdateType GetMessageUpdateType()
        {
            if (message.ForwardFromUserOrNull.IsNotNull())
            {
                return EChatUpdateType.ForwardMessage;
            }

            if (message.ReplyToMessageOrNull.IsNotNull())
            {
                return EChatUpdateType.ToReplyMessage;
            }

            return EChatUpdateType.Message;
        }
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

    private async Task<ChatUpdateMessage> CreateChatUpdateMessageAsync(long chatId, Message message, CancellationToken cancellationToken)
    {
        ChatUpdateForwardFromUser forwardFromUserOrNull = null;

        if (message!.ForwardFromMessageId.IsNotNull()
            || message!.ForwardSignature.IsNotNull()
            || message!.ForwardSenderName.IsNotNull()
            || message!.ForwardDate.IsNotNull()
           )
        {
            TelegramUser forwardUser = null;
            var forwardFromBot = false;

            if (message!.ForwardFrom.IsNotNull())
            {
                var tgUser = message!.ForwardFrom;

                forwardUser = CreateUser(tgUser);
                forwardFromBot = tgUser!.IsBot;
            }

            forwardFromUserOrNull = new(
                forwardUser,
                forwardFromBot,
                CreateChat(message.ForwardFromChat),
                message.ForwardFromMessageId,
                message.ForwardSignature,
                message.ForwardSenderName,
                message.ForwardDate
                );
        }

        ChatMessageReplyToMessage replyToMessageOrNull = null;

        if (message.ReplyToMessage.IsNotNull())
        {
            var replyToMessage = message.ReplyToMessage;
            replyToMessageOrNull = new(
                CreateChat(replyToMessage?.Chat),
                replyToMessage?.MessageId,
                replyToMessage?.Text,
                await DownloadMessagePhotoAsync(chatId, replyToMessage, cancellationToken),
                await DownloadMessageDocumentAsync(chatId, replyToMessage, cancellationToken),
                CreateChat(replyToMessage?.SenderChat),
                CreateLocation(replyToMessage?.Location)
                );
        }

        return new(
            CreateChat(message.Chat),
            message.MessageId,
            message.Text,
            replyToMessageOrNull,
            await DownloadMessagePhotoAsync(chatId, message, cancellationToken),
            await DownloadMessageDocumentAsync(chatId, message, cancellationToken),
            forwardFromUserOrNull,
            CreateChat(message.SenderChat),
            CreateLocation(message.Location)
            );
    }

    private static TelegramChat CreateChat(Chat chat)
    {
        if (chat.IsNull())
        {
            return default;
        }

        TelegramChatPhoto telegramChatPhoto = null;

        if (chat.Photo.IsNotNull())
        {
            var photo = chat.Photo;
            telegramChatPhoto = new(photo!.SmallFileId, photo.SmallFileUniqueId, photo.BigFileId, photo.BigFileUniqueId);
        }

        TelegramChatPermissions telegramChatPermissions = null;

        if (chat.Permissions.IsNotNull())
        {
            var permissions = chat.Permissions;

            telegramChatPermissions = new(
                permissions!.CanSendMessages,
                permissions.CanSendAudios,
                permissions.CanSendDocuments,
                permissions.CanSendPhotos,
                permissions.CanSendVideos,
                permissions.CanSendVideoNotes,
                permissions.CanSendVoiceNotes,
                permissions.CanSendPolls,
                permissions.CanSendOtherMessages,
                permissions.CanAddWebPagePreviews,
                permissions.CanChangeInfo,
                permissions.CanInviteUsers,
                permissions.CanPinMessages,
                permissions.CanManageTopics
                );
        }
        
        return new(
            chat.Id,
            chat.Type,
            chat.Title,
            chat.Username,
            chat.FirstName,
            chat.LastName,
            chat.IsForum,
            telegramChatPhoto,
            chat.ActiveUsernames,
            chat.EmojiStatusCustomEmojiId,
            chat.Bio,
            chat.HasPrivateForwards,
            chat.HasRestrictedVoiceAndVideoMessages,
            chat.JoinToSendMessages,
            chat.JoinByRequest,
            chat.Description,
            chat.InviteLink,
            telegramChatPermissions,
            chat.SlowModeDelay,
            chat.MessageAutoDeleteTime,
            chat.HasAggressiveAntiSpamEnabled,
            chat.HasHiddenMembers,
            chat.HasProtectedContent,
            chat.StickerSetName,
            chat.CanSetStickerSet,
            chat.LinkedChatId,
            CreateChatLocation(chat.Location)
            );
    }

    private static TelegramUser CreateUser(User user)
    {
        if (user.IsNull())
        {
            return default;
        }

        return new(
            user.Id,
            user.Username ?? "",
            user.FirstName,
            user.LastName ?? "",
            user.IsBot,
            user.LanguageCode,
            user.IsPremium,
            user.AddedToAttachmentMenu,
            user.CanJoinGroups,
            user.CanReadAllGroupMessages,
            user.SupportsInlineQueries
            );
    }

    private static ChatUpdateMemberUpdate CreateChatMember(ChatMemberUpdated chatMember)
    {
        var fromUser = CreateUser(chatMember!.From);

        return new(
            CreateChat(chatMember.Chat),
            fromUser,
            chatMember.Date,
            CreateChatMessageMember(chatMember.OldChatMember),
            CreateChatMessageMember(chatMember.NewChatMember)
            );
    }

    private static ChatMessageMember CreateChatMessageMember(ChatMember chatMember)
        => new(chatMember.Status);

    private static TelegramChatLocation CreateChatLocation(ChatLocation location)
    {
        if (location.IsNull())
        {
            return new(null, "");
        }

        return new(CreateLocation(location.Location), location.Address);
    }

    private static TelegramLocation CreateLocation(Location location)
    {
        if (location.IsNull())
        {
            return default;
        }

        return new(
            location.Longitude,
            location.Latitude,
            location.HorizontalAccuracy,
            location.LivePeriod,
            location.Heading,
            location.ProximityAlertRadius
            );
    }

    
}