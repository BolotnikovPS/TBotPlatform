#nullable enable
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    public Task<Message> SendOrUpdateTextMessage(
        string text,
        InlineMarkupList inlineMarkupList,
        FileDataBase photoData,
        bool disableNotification,
        CancellationToken cancellationToken
        )
    {
        InlineKeyboardMarkup? inlineKeyboard = null;

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

        return SendOrUpdateTextMessage(text, inlineKeyboard, photoData, disableNotification, cancellationToken);
    }

    public Task<Message> SendOrUpdateTextMessage(
        string text,
        InlineMarkupList inlineMarkupList,
        FileDataBase photoData,
        CancellationToken cancellationToken
        ) => SendOrUpdateTextMessage(text, inlineMarkupList, photoData, disableNotification: false, cancellationToken);

    public Task<Message> SendOrUpdateTextMessage(
        string text,
        InlineMarkupList inlineMarkupList,
        bool disableNotification,
        CancellationToken cancellationToken
        )
    {
        InlineKeyboardMarkup? inlineKeyboard = null;

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

        return SendOrUpdateTextMessage(text, inlineKeyboard, photoData: null, disableNotification, cancellationToken);
    }

    public Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupList inlineMarkupList, CancellationToken cancellationToken)
        => SendOrUpdateTextMessage(text, inlineMarkupList, disableNotification: false, cancellationToken);

    public Task<Message> SendOrUpdateTextMessage(
        string text,
        InlineMarkupMassiveList? inlineMarkupMassiveList,
        bool disableNotification,
        CancellationToken cancellationToken
        )
    {
        InlineKeyboardMarkup? inlineKeyboard = null;

        if (inlineMarkupMassiveList.CheckAny())
        {
            inlineKeyboard = Map(inlineMarkupMassiveList);
        }

        return SendOrUpdateTextMessage(text, inlineKeyboard, photoData: null, disableNotification, cancellationToken);
    }

    public Task<Message> SendOrUpdateTextMessage(string text, InlineMarkupMassiveList inlineMarkupMassiveList, CancellationToken cancellationToken)
        => SendOrUpdateTextMessage(text, inlineMarkupMassiveList, disableNotification: false, cancellationToken);

    public Task<Message> SendOrUpdateTextMessage(string text, bool disableNotification, CancellationToken cancellationToken)
        => SendOrUpdateTextMessage(
            text,
            inlineMarkupMassiveList: null,
            disableNotification,
            cancellationToken
            );

    public Task<Message> SendOrUpdateTextMessage(string text, CancellationToken cancellationToken)
        => SendOrUpdateTextMessage(
            text,
            inlineMarkupMassiveList: null,
            disableNotification: false,
            cancellationToken
            );

    private async Task<Message> SendOrUpdateTextMessage(
        string text,
        InlineKeyboardMarkup? inlineKeyboard,
        FileDataBase? photoData,
        bool disableNotification,
        CancellationToken cancellationToken
        )
    {
        ChatIdValidOrThrow();
        TextLengthValidOrThrow(text);

        if (photoData.IsNotNull())
        {
            var checkToDelete = ChatUpdate.IsNotNull()
                                && ChatUpdate!.CallbackQuery.IsNotNull()
                                && (DateTime.UtcNow - ChatUpdate.CallbackQuery!.Message!.Date).TotalDays < 1;

            if (checkToDelete)
            {
                CallbackQueryValidOrThrow(ChatUpdate?.CallbackQuery);

                await telegramContext.DeleteMessage(ChatId, ChatUpdate!.CallbackQuery!.Message!.MessageId, cancellationToken);
            }

            await using var fileStream = new MemoryStream(photoData!.Bytes);
            return await telegramContext.SendPhoto(
                ChatId,
                InputFile.FromStream(fileStream),
                text,
                ParseMode,
                replyMarkup: inlineKeyboard,
                disableNotification: disableNotification,
                cancellationToken: cancellationToken
                );
        }

        if (MarkupNextState.IsNull())
        {
            return await telegramContext.SendMessage(
                ChatId,
                text,
                ParseMode,
                replyMarkup: inlineKeyboard,
                disableNotification: disableNotification,
                cancellationToken: cancellationToken
                );
        }

        CallbackQueryValidOrThrow(ChatUpdate!.CallbackQuery);

        return ChatUpdate.CallbackQuery.WithPhoto()
            ? await telegramContext.EditMessageCaption(
                ChatId,
                ChatUpdate.CallbackQuery!.Message!.Id,
                text,
                ParseMode,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
                )
            : await telegramContext.EditMessageText(
                ChatId,
                ChatUpdate.CallbackQuery!.Message!.MessageId,
                text,
                ParseMode,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
                );
    }
}