using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using TBotPlatform.Contracts.Bots.Locations;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.Markups.InlineMarkups;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Handlers;

internal partial class TelegramChatHandler
{
    private async Task<ChatMessage> CreateChatUpdateMessageAsync(long chatId, Message message, CancellationToken cancellationToken)
    {
        ChatForward forwardOrNull = null;
        
        if (message!.ForwardFromMessageId.IsNotNull()
            || message!.ForwardSignature.IsNotNull()
            || message!.ForwardSenderName.IsNotNull()
            || message!.ForwardDate.IsNotNull()
           )
        {
            forwardOrNull = new(
                CreateUser(message!.ForwardFrom),
                message.ForwardFrom?.IsBot ?? false,
                CreateChat(message.ForwardFromChat),
                message.ForwardFromMessageId,
                message.ForwardSignature,
                message.ForwardSenderName,
                message.ForwardDate
                );
        }

        ChatReplyToMessage replyToMessageOrNull = null;

        if (message.ReplyToMessage.IsNotNull())
        {
            var replyToMessage = message.ReplyToMessage;
            replyToMessageOrNull = new(
                CreateChat(replyToMessage!.Chat),
                replyToMessage.MessageId,
                replyToMessage.Text,
                await DownloadMessagePhotoAsync(chatId, replyToMessage, cancellationToken),
                await DownloadMessageDocumentAsync(chatId, replyToMessage, cancellationToken),
                CreateChat(replyToMessage.SenderChat),
                CreateLocation(replyToMessage.Location)
                );
        }

        return new(
            GetMessageUpdateType(),
            CreateChat(message.Chat),
            message.MessageId,
            message.Date,
            message.Text,
            replyToMessageOrNull,
            await DownloadMessagePhotoAsync(chatId, message, cancellationToken),
            await DownloadMessageDocumentAsync(chatId, message, cancellationToken),
            forwardOrNull,
            CreateChat(message.SenderChat),
            CreateLocation(message.Location),
            CreateInlineMarkupList(message.ReplyMarkup),
            CreateWebAppData(message.WebAppData),
            CreateChatMessageEntity(message.Entities)
            );

        EChatMessageType GetMessageUpdateType()
        {
            if (forwardOrNull.IsNotNull())
            {
                return EChatMessageType.ForwardMessage;
            }

            return replyToMessageOrNull.IsNotNull() ? EChatMessageType.ToReplyMessage : EChatMessageType.Message;
        }
    }

    private static TelegramChat CreateChat(Chat chat)
    {
        if (chat.IsNull())
        {
            return default;
        }

        return new(
            chat.Id,
            CreateChatType(chat.Type),
            chat.Title,
            chat.Username,
            chat.FirstName,
            chat.LastName,
            chat.IsForum
            );
    }

    private static TelegramChatFullInfo CreateChat(ChatFullInfo chat)
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

        TelegramChatPinMessage telegramPinMessage = null;

        if (chat.PinnedMessage.IsNotNull())
        {
            var pinMessage = chat.PinnedMessage;

            telegramPinMessage = new(pinMessage.MessageId, pinMessage.Date, pinMessage.Text);
        }

        return new(
            chat.Id,
            CreateChatType(chat.Type),
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
            telegramPinMessage,
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

    private static ChatMemberUpdate CreateChatMember(ChatMemberUpdated chatMember)
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

    private static ChatMemberData CreateChatMessageMember(ChatMember chatMember)
    {
        if (chatMember.IsNull())
        {
            return new(EChatMemberStatus.None);
        }

        var status = chatMember.Status switch
        {
            ChatMemberStatus.Creator => EChatMemberStatus.Creator,
            ChatMemberStatus.Administrator => EChatMemberStatus.Administrator,
            ChatMemberStatus.Member => EChatMemberStatus.Member,
            ChatMemberStatus.Left => EChatMemberStatus.Left,
            ChatMemberStatus.Kicked => EChatMemberStatus.Kicked,
            ChatMemberStatus.Restricted => EChatMemberStatus.Restricted,
            _ => EChatMemberStatus.None,
        };


        return new(status, CreateUser(chatMember.User));
    }

    private static TelegramChatLocation CreateChatLocation(ChatLocation location)
    {
        if (location.IsNull())
        {
            return new(location: null, "");
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

    private static InlineMarkupList CreateInlineMarkupList(InlineKeyboardMarkup replyMarkup)
    {
        if (replyMarkup.IsNull())
        {
            return default;
        }

        var result = new InlineMarkupList();

        var inlineMarkups = replyMarkup.InlineKeyboard.SelectMany(x => x.Select(CreateInlineMarkup));

        foreach (var inlineMarkup in inlineMarkups)
        {
            result.Add(inlineMarkup);
        }

        return result;
    }

    private static InlineMarkupBase CreateInlineMarkup(InlineKeyboardButton button)
    {
        if (button.CallbackData.IsNotNull())
        {
            if (button.CallbackData!.TryParseJson<MarkupNextState>(out var newMarkupNextState))
            {
                return new InlineMarkupState(button.Text, newMarkupNextState);
            }
        }

        if (button.Url.IsNotNull())
        {
            return new InlineMarkupUrl(button.Text, button.Url);
        }

        if (button.LoginUrl.IsNotNull())
        {
            var loginUrl = button.LoginUrl;
            return new InlineMarkupLoginUrl(
                button.Text,
                loginUrl?.Url,
                loginUrl?.ForwardText,
                loginUrl?.BotUsername,
                loginUrl?.RequestWriteAccess ?? false
                );
        }

        if (button.CallbackGame.IsNotNull())
        {
            return new InlineMarkupCallBackGame(button.Text);
        }

        if (button.Pay)
        {
            return new InlineMarkupPayment(button.Text);
        }

        if (button.SwitchInlineQuery.IsNotNull())
        {
            return new InlineMarkupSwitchInlineQuery(button.Text, button.SwitchInlineQuery);
        }

        if (button.SwitchInlineQueryChosenChat.IsNotNull())
        {
            var switchInlineQueryChosenChat = button.SwitchInlineQueryChosenChat;
            return new InlineMarkupSwitchInlineQueryChosenChat(
                button.Text,
                switchInlineQueryChosenChat?.Query,
                switchInlineQueryChosenChat?.AllowUserChats ?? false,
                switchInlineQueryChosenChat?.AllowBotChats ?? false,
                switchInlineQueryChosenChat?.AllowGroupChats ?? false,
                switchInlineQueryChosenChat?.AllowChannelChats ?? false
                );
        }

        if (button.SwitchInlineQueryCurrentChat.IsNotNull())
        {
            return new InlineMarkupSwitchInlineQueryCurrentChat(button.Text, button.SwitchInlineQueryCurrentChat);
        }

        if (button.WebApp.IsNotNull())
        {
            return new InlineMarkupWebApp(button.Text, button.WebApp!.Url);
        }

        if (button.CopyText.IsNotNull())
        {
            return new InlineMarkupCopyText(button.Text, button.CopyText?.Text);
        }

        return new InlineMarkupStateDefault(button.Text);
    }

    private static ChatWebAppData CreateWebAppData(WebAppData data)
    {
        if (data.IsNull())
        {
            return default;
        }

        return new(data.Data, data.ButtonText);
    }

    private static ChatEntity[] CreateChatMessageEntity(MessageEntity[] entity)
    {
        if (entity.IsNull()
            || !entity.CheckAny()
            )
        {
            return default;
        }

        return entity
              .Select(
                   z => new ChatEntity(
                       CreateChatMessageEntityType(z.Type),
                       z.Offset,
                       z.Length,
                       z.Url,
                       CreateUser(z.User)
                       )
                   )
              .ToArray();
    }

    private static ChatShippingAddress CreateChatMessageShippingAddress(ShippingAddress shippingAddress)
    {
        if (shippingAddress.IsNull())
        {
            return default;
        }

        return new(
            shippingAddress!.CountryCode,
            shippingAddress.State,
            shippingAddress.City,
            shippingAddress.StreetLine1,
            shippingAddress.StreetLine2,
            shippingAddress.PostCode
            );
    }
}