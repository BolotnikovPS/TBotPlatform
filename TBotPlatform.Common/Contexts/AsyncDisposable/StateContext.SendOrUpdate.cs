﻿using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, FileDataBase photoData, bool disableNotification, CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup inlineKeyboard = null;

        if (inlineMarkupList.CheckAny())
        {
            inlineKeyboard = Map(
                new InlineMarkupMassiveList
                {
                    new()
                    {
                        InlineMarkups = inlineMarkupList,
                        ButtonsPerRow = 1,
                    },
                });
        }

        return SendOrUpdateTextMessageAsync(text, inlineKeyboard, photoData, disableNotification, cancellationToken);
    }

    public Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, FileDataBase photoData, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(text, inlineMarkupList, photoData, disableNotification: false, cancellationToken);

    public Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, bool disableNotification, CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup inlineKeyboard = null;

        if (inlineMarkupList.CheckAny())
        {
            inlineKeyboard = Map(
                new InlineMarkupMassiveList
                {
                    new()
                    {
                        InlineMarkups = inlineMarkupList,
                        ButtonsPerRow = 1,
                    },
                });
        }

        return SendOrUpdateTextMessageAsync(text, inlineKeyboard, photoData: null, disableNotification, cancellationToken);
    }

    public Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(text, inlineMarkupList, disableNotification: false, cancellationToken);

    public Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupMassiveList inlineMarkupMassiveList, bool disableNotification, CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup inlineKeyboard = null;

        if (inlineMarkupMassiveList.CheckAny())
        {
            inlineKeyboard = Map(inlineMarkupMassiveList);
        }

        return SendOrUpdateTextMessageAsync(text, inlineKeyboard, photoData: null, disableNotification, cancellationToken);
    }

    public Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineMarkupMassiveList inlineMarkupMassiveList, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(text, inlineMarkupMassiveList, disableNotification: false, cancellationToken);

    public Task<ChatResult> SendOrUpdateTextMessageAsync(string text, bool disableNotification, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(
            text,
            inlineMarkupMassiveList: null,
            disableNotification,
            cancellationToken
            );

    public Task<ChatResult> SendOrUpdateTextMessageAsync(string text, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(
            text,
            inlineMarkupMassiveList: null,
            disableNotification: false,
            cancellationToken
            );

    private async Task<ChatResult> SendOrUpdateTextMessageAsync(string text, InlineKeyboardMarkup inlineKeyboard, FileDataBase photoData, bool disableNotification, CancellationToken cancellationToken)
    {
        if (chatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (text.IsNull())
        {
            return default;
        }

        if (text.Length > StateContextConstant.TextLength)
        {
            throw new TextLengthException(text.Length, StateContextConstant.TextLength);
        }

        if (photoData.IsNotNull())
        {
            var checkToDelete = ChatUpdate.CallbackQueryOrNull.IsNotNull()
                                && (DateTime.UtcNow - ChatUpdate.CallbackQueryOrNull!.Date).TotalDays < 1;

            if (checkToDelete)
            {
                if (ChatUpdate.CallbackQueryOrNull.IsNull())
                {
                    throw new CallbackQueryMessageIdOrNullArgException();
                }

                await telegramContext.DeleteMessageAsync(chatId, ChatUpdate.CallbackQueryOrNull.MessageId, cancellationToken);
            }

            await using var fileStream = new MemoryStream(photoData.Bytes);
            var resultSendPhoto = await telegramContext.SendPhotoAsync(
                chatId,
                InputFile.FromStream(fileStream),
                text,
                inlineKeyboard,
                disableNotification,
                cancellationToken
                );

            return telegramMapping.MessageToResult(resultSendPhoto);
        }

        if (MarkupNextState.IsNull())
        {
            var resultSendText = await telegramContext.SendTextMessageAsync(
                chatId,
                text,
                inlineKeyboard,
                disableNotification,
                cancellationToken
                );

            return telegramMapping.MessageToResult(resultSendText);
        }

        if (ChatUpdate.CallbackQueryOrNull.IsNull())
        {
            throw new CallbackQueryMessageIdOrNullArgException();
        }

        var result = ChatUpdate.CallbackQueryOrNull!.MessageWithImage
            ? await telegramContext.EditMessageCaptionAsync(chatId, ChatUpdate.CallbackQueryOrNull.MessageId, text, inlineKeyboard, cancellationToken)
            : await telegramContext.EditMessageTextAsync(chatId, ChatUpdate.CallbackQueryOrNull.MessageId, text, inlineKeyboard, cancellationToken);

        return telegramMapping.MessageToResult(result);
    }
}