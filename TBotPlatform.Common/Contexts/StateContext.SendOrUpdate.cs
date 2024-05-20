using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext<T>
{
    public Task<Message> SendOrUpdateTextMessageAsync(
        string text,
        InlineMarkupList inlineMarkupList,
        FileData photoData,
        CancellationToken cancellationToken
        )
    {
        InlineKeyboardMarkup inlineKeyboard = null;

        if (inlineMarkupList.CheckAny())
        {
            inlineKeyboard = Map(
                new InlineMarkupMassivList
                {
                    new()
                    {
                        InlineMarkups = inlineMarkupList,
                        ButtonsPerRow = 1,
                    },
                });
        }

        return SendOrUpdateTextMessageAsync(text, inlineKeyboard, photoData, cancellationToken);
    }

    public Task<Message> SendOrUpdateTextMessageAsync(
        string text,
        InlineMarkupList inlineMarkupList,
        CancellationToken cancellationToken
        )
    {
        InlineKeyboardMarkup inlineKeyboard = null;

        if (inlineMarkupList.CheckAny())
        {
            inlineKeyboard = Map(
                new InlineMarkupMassivList
                {
                    new()
                    {
                        InlineMarkups = inlineMarkupList,
                        ButtonsPerRow = 1,
                    },
                });
        }

        return SendOrUpdateTextMessageAsync(text, inlineKeyboard, null, cancellationToken);
    }

    public Task<Message> SendOrUpdateTextMessageAsync(
        string text,
        InlineMarkupMassivList inlineMarkupMassivList,
        CancellationToken cancellationToken
        )
    {
        InlineKeyboardMarkup inlineKeyboard = null;

        if (inlineMarkupMassivList.CheckAny())
        {
            inlineKeyboard = Map(inlineMarkupMassivList);
        }

        return SendOrUpdateTextMessageAsync(text, inlineKeyboard, null, cancellationToken);
    }

    public Task<Message> SendOrUpdateTextMessageAsync(
        string text,
        CancellationToken cancellationToken
        ) => SendOrUpdateTextMessageAsync(
        text,
        inlineMarkupMassivList: null,
        cancellationToken
        );

    private async Task<Message> SendOrUpdateTextMessageAsync(
        string text,
        InlineKeyboardMarkup inlineKeyboard,
        FileData photoData,
        CancellationToken cancellationToken
        )
    {
        if (!UserDb.ChatId.CheckAny())
        {
            throw new ChatIdArgException();
        }

        if (!text.CheckAny())
        {
            return default;
        }
        
        if (text.Length > TextLength)
        {
            throw new TextLengthException();
        }

        var checkToEdit = MarkupNextState.CheckAny()
                          && ChatMessage.CallbackQueryDateIdOrNull.CheckAny()
                          && (DateTime.UtcNow - ChatMessage.CallbackQueryDateIdOrNull!.Value).TotalDays < 1;

        if (photoData.CheckAny())
        {
            if (checkToEdit)
            {
                await botClient.DeleteMessageAsync(
                    UserDb.ChatId,
                    ChatMessage.CallbackQueryMessageIdOrNull!.Value,
                    cancellationToken
                    );
            }

            await using var fileStream = new MemoryStream(photoData.Byte);
            return await botClient.SendPhotoAsync(
                UserDb.ChatId,
                InputFile.FromStream(fileStream),
                caption: text,
                parseMode: ParseMode,
                protectContent: ProtectContent,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
                );
        }

        if (!checkToEdit)
        {
            return await botClient.SendTextMessageAsync(
                UserDb.ChatId,
                text,
                parseMode: ParseMode,
                protectContent: ProtectContent,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
                );
        }

        if (!ChatMessage.CallbackQueryMessageIdOrNull.HasValue)
        {
            throw new CallbackQueryMessageIdOrNullArgException();
        }

        if (ChatMessage.CallbackQueryMessageWithCaption)
        {
            return await botClient.EditMessageCaptionAsync(
                UserDb.ChatId,
                ChatMessage.CallbackQueryMessageIdOrNull.Value,
                text,
                ParseMode,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
                );
        }

        return await botClient.EditMessageTextAsync(
            UserDb.ChatId,
            ChatMessage.CallbackQueryMessageIdOrNull.Value,
            text,
            ParseMode,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken
            );
    }
}