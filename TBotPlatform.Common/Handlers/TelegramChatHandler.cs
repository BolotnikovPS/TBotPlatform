using Microsoft.Extensions.Logging;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Handlers;

internal partial class TelegramChatHandler(ILogger<TelegramChatHandler> logger, ITelegramContext botClient)
    : ITelegramUpdateHandler, ITelegramMappingHandler
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
            return new(EChatUpdateType.None, new(EChatMessageType.None, null!, messageId: 0, DateTime.UtcNow, text: null, replyToMessageOrNull: null, photoData: null, documentData: null));
        }

        var message = new ChatMessage(EChatMessageType.None, null!, messageId: 0, DateTime.UtcNow, text: null, replyToMessageOrNull: null, photoData: null, documentData: null);
        ChatMessage editMessageOrNull = null;
        ChatMessage channelPostOrNull = null;
        ChatMessage editChannelPostOrNull = null;
        ChatCallbackQuery callbackQueryOrNull = null;
        ChatMemberUpdate myMemberUpdateOrNull = null;
        ChatMemberUpdate memberUpdateOrNull = null;
        ChatJoinRequestQuery joinRequestOrNull = null;
        ChatInlineQuery inlineQueryOrNull = null;
        ChatChosenInlineResult chosenInlineResultOrNull = null;
        ChatPoll chatUpdatePollOrNull = null;
        ChatPollAnswer pollAnswerOrNull = null;
        ChatShippingQuery shippingQueryOrNull = null;
        ChatPreCheckoutQuery preCheckoutQueryOrNull = null;

        try
        {
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
                                CreateChatType(inlineQuery.ChatType)
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
                            ChatPollOption[] optionsOrNull = null;

                            if (pool!.Options.IsNotNull())
                            {
                                optionsOrNull = pool
                                               .Options
                                               .Select(z => new ChatPollOption(z.Text, z.VoterCount))
                                               .ToArray();
                            }

                            ChatEntity[] explanationEntitiesOrNull = null;

                            if (pool.ExplanationEntities.IsNotNull())
                            {
                                explanationEntitiesOrNull = CreateChatMessageEntity(pool.ExplanationEntities);
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

                            shippingQueryOrNull = new(
                                shippingQuery!.Id,
                                CreateUser(shippingQuery.From),
                                shippingQuery.InvoicePayload,
                                CreateChatMessageShippingAddress(shippingQuery!.ShippingAddress));
                        }
                    }
                    break;

                case UpdateType.PreCheckoutQuery:
                    {
                        if (update.PreCheckoutQuery.IsNotNull())
                        {
                            var pre = update.PreCheckoutQuery;
                            ChatOrderInfo orderInfoOrNull = null;

                            if (pre!.OrderInfo.IsNotNull())
                            {
                                var orderInfo = pre!.OrderInfo;

                                orderInfoOrNull = new(orderInfo!.Name, orderInfo.PhoneNumber, orderInfo.Email, CreateChatMessageShippingAddress(orderInfo!.ShippingAddress));
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
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка формирования сообщения с telegram");
        }

        var chatUpdateType = update.Type switch
        {
            UpdateType.Message => EChatUpdateType.Message,
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
    }

    public ChatResult MessageToResult(Message message)
        => new(
            CreateChat(message.Chat),
            message.MessageId,
            message.Text,
            CreateInlineMarkupList(message.ReplyMarkup),
            CreateChatMessageEntity(message.Entities)
            );

    public ChatMemberData ChatMemberToData(ChatMember chatMember) => CreateChatMessageMember(chatMember);

    public TelegramChat ChatToTelegramChat(Chat chat) => CreateChat(chat);
}