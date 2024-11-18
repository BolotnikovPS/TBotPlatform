using System.Text;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Account;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.State;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext(ITelegramMappingHandler telegramMapping, ITelegramContext telegramContext, long chatId) 
    : IStateContextBase
{
    public StateResult StateResult { get; set; }
    public MarkupNextState MarkupNextState { get; private set; }
    public ChatUpdate ChatUpdate { get; private set; }

    internal long ChatId { get; set; } = chatId;

    private const string ChooseAction = "😊 Выберите действие";

    public void CreateStateContext(ChatUpdate chatUpdate, MarkupNextState markupNextState)
    {
        ChatIdValidOrThrow();

        StateResult = new();
        MarkupNextState = markupNextState;
        ChatUpdate = chatUpdate;
    }

    public Guid GetCurrentOperation() => telegramContext.GetCurrentOperation();

    public ITelegramContext GetTelegramContext() => telegramContext;

    public async Task<ChatResult> SendDocumentAsync(FileDataBase documentData, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        await using var fileStream = new MemoryStream(documentData.Bytes);

        var inputFile = InputFile.FromStream(fileStream, documentData.Name);

        var result = await telegramContext.SendDocumentAsync(
            ChatId,
            inputFile,
            disableNotification,
            cancellationToken
            );

        return telegramMapping.MessageToResult(result);
    }

    public Task<ChatResult> SendDocumentAsync(FileDataBase documentData, CancellationToken cancellationToken)
        => SendDocumentAsync(documentData, disableNotification: false, cancellationToken);

    public async Task<ChatResult> SendPhotoAsync(FileDataBase documentData, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        await using var fileStream = new MemoryStream(documentData.Bytes);

        var inputFile = InputFile.FromStream(fileStream, documentData.Name);

        var result = await telegramContext.SendPhotoAsync(
            ChatId,
            inputFile,
            disableNotification,
            cancellationToken
            );

        return telegramMapping.MessageToResult(result);
    }

    public Task<ChatResult> SendPhotoAsync(FileDataBase documentData, CancellationToken cancellationToken)
        => SendPhotoAsync(documentData, disableNotification: false, cancellationToken);

    public Task SendChatActionAsync(ChatAction chatAction, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        return telegramContext.SendChatActionAsync(ChatId, chatAction, cancellationToken);
    }

    public async Task<ChatResult> RemoveMarkupAsync(string text, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        TextLengthValidOrThrow(text);

        var result = await telegramContext.SendTextMessageAsync(
            ChatId,
            text,
            new ReplyKeyboardRemove(),
            disableNotification: false,
            cancellationToken
            );

        return telegramMapping.MessageToResult(result);
    }

    public Task<ChatResult> UpdateMainButtonsAsync(MainButtonMassiveList mainButtonMassiveList, CancellationToken cancellationToken)
        => UpdateMainButtonsAsync(mainButtonMassiveList, ChooseAction, cancellationToken);

    public async Task<ChatResult> UpdateMainButtonsAsync(MainButtonMassiveList mainButtonMassiveList, string text, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        TextLengthValidOrThrow(text);

        var newMarkup = Map(mainButtonMassiveList);

        if (newMarkup.IsNull())
        {
            throw new ReplyKeyboardMarkupArgException();
        }

        var result = await telegramContext.SendTextMessageAsync(
            ChatId,
            text,
            newMarkup,
            disableNotification: false,
            cancellationToken
            );

        return telegramMapping.MessageToResult(result);
    }

    public Task UpdateMarkupTextAndDropButtonAsync(string text, CancellationToken cancellationToken)
    {
        if (MarkupNextState.State.IsNull())
        {
            return Task.CompletedTask;
        }

        ChatIdValidOrThrow();
        CallbackQueryValidOrThrow(ChatUpdate.CallbackQueryOrNull);
        TextLengthValidOrThrow(text);

        var message = new StringBuilder(ChatUpdate.CallbackQueryOrNull!.Text)
                     .AppendLine()
                     .AppendLine(text)
                     .ToString();

        TextLengthValidOrThrow(message);

        if (ChatUpdate.CallbackQueryOrNull!.MessageWithImage)
        {
            return telegramContext.EditMessageCaptionAsync(
                ChatId,
                ChatUpdate.CallbackQueryOrNull!.MessageId,
                message,
                cancellationToken
                );
        }

        return telegramContext.EditMessageTextAsync(
            ChatId,
            ChatUpdate.CallbackQueryOrNull!.MessageId,
            message,
            cancellationToken
            );
    }

    public Task UpdateMarkupTextAndDropButtonAsync(CancellationToken cancellationToken)
        => UpdateMarkupTextAndDropButtonAsync("", cancellationToken);

    public Task RemoveMessageAsync(int messageId, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        MessageIdValidOrThrow(messageId);
        
        return telegramContext.DeleteMessageAsync(ChatId, messageId, cancellationToken);
    }

    public async Task<TelegramBusinessInfo> GetBusinessInfoAsync(CancellationToken cancellationToken)
    {
        var result = await telegramContext.GetBusinessConnectionAsync(cancellationToken);

        return telegramMapping.GetTelegramBusinessInfo(result);
    }

    public ValueTask DisposeAsync()
    {
        ChatUpdate = null;
        MarkupNextState = null;
        StateResult = null;

        return ValueTask.CompletedTask;
    }
}