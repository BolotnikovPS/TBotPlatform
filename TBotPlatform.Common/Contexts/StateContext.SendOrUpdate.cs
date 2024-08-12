using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext
{
    public Task<Message> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, FileData photoData, bool disableNotification, CancellationToken cancellationToken)
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

    public Task<Message> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, FileData photoData, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(text, inlineMarkupList, photoData, false, cancellationToken);

    public Task<Message> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, bool disableNotification, CancellationToken cancellationToken)
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

        return SendOrUpdateTextMessageAsync(text, inlineKeyboard, null, disableNotification, cancellationToken);
    }

    public Task<Message> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(text, inlineMarkupList, false, cancellationToken);

    public Task<Message> SendOrUpdateTextMessageAsync(string text, InlineMarkupMassiveList inlineMarkupMassiveList, bool disableNotification, CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup inlineKeyboard = null;

        if (inlineMarkupMassiveList.CheckAny())
        {
            inlineKeyboard = Map(inlineMarkupMassiveList);
        }

        return SendOrUpdateTextMessageAsync(text, inlineKeyboard, null, disableNotification, cancellationToken);
    }

    public Task<Message> SendOrUpdateTextMessageAsync(string text, InlineMarkupMassiveList inlineMarkupMassiveList, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(text, inlineMarkupMassiveList, false, cancellationToken);

    public Task<Message> SendOrUpdateTextMessageAsync(string text, bool disableNotification, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(
            text,
            inlineMarkupMassiveList: null,
            disableNotification,
            cancellationToken
            );

    public Task<Message> SendOrUpdateTextMessageAsync(string text, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(
            text,
            inlineMarkupMassiveList: null,
            false,
            cancellationToken
            );

    private async Task<Message> SendOrUpdateTextMessageAsync(string text, InlineKeyboardMarkup inlineKeyboard, FileData photoData, bool disableNotification, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
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

                await botClient.DeleteMessageAsync(ChatId, ChatUpdate.CallbackQueryOrNull.MessageId, cancellationToken);
            }

            await using var fileStream = new MemoryStream(photoData.Byte);
            return await botClient.SendPhotoAsync(
                ChatId,
                InputFile.FromStream(fileStream),
                text,
                inlineKeyboard,
                disableNotification,
                cancellationToken
                );
        }

        if (MarkupNextState.IsNull())
        {
            return await botClient.SendTextMessageAsync(
                ChatId,
                text,
                inlineKeyboard,
                disableNotification,
                cancellationToken
                );
        }

        if (ChatUpdate.CallbackQueryOrNull.IsNull())
        {
            throw new CallbackQueryMessageIdOrNullArgException();
        }

        return ChatUpdate.CallbackQueryOrNull!.MessageWithImage
            ? await botClient.EditMessageCaptionAsync(ChatId, ChatUpdate.CallbackQueryOrNull.MessageId, text, inlineKeyboard, cancellationToken)
            : await botClient.EditMessageTextAsync(ChatId, ChatUpdate.CallbackQueryOrNull.MessageId, text, inlineKeyboard, cancellationToken);
    }
}