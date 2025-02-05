#nullable enable
using TBotPlatform.Contracts;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    public Task<Message> SendOrUpdateTextMessageAsync(
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

        return SendOrUpdateTextMessageAsync(text, inlineKeyboard, photoData, disableNotification, cancellationToken);
    }

    public Task<Message> SendOrUpdateTextMessageAsync(
        string text,
        InlineMarkupList inlineMarkupList,
        FileDataBase photoData,
        CancellationToken cancellationToken
        ) => SendOrUpdateTextMessageAsync(text, inlineMarkupList, photoData, disableNotification: false, cancellationToken);

    public Task<Message> SendOrUpdateTextMessageAsync(
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

        return SendOrUpdateTextMessageAsync(text, inlineKeyboard, photoData: null, disableNotification, cancellationToken);
    }

    public Task<Message> SendOrUpdateTextMessageAsync(string text, InlineMarkupList inlineMarkupList, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(text, inlineMarkupList, disableNotification: false, cancellationToken);

    public Task<Message> SendOrUpdateTextMessageAsync(
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

        return SendOrUpdateTextMessageAsync(text, inlineKeyboard, photoData: null, disableNotification, cancellationToken);
    }

    public Task<Message> SendOrUpdateTextMessageAsync(string text, InlineMarkupMassiveList inlineMarkupMassiveList, CancellationToken cancellationToken)
        => SendOrUpdateTextMessageAsync(text, inlineMarkupMassiveList, disableNotification: false, cancellationToken);

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
            disableNotification: false,
            cancellationToken
            );

    private async Task<Message> SendOrUpdateTextMessageAsync(
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

        return ChatUpdate.CallbackQuery.WithImage()
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