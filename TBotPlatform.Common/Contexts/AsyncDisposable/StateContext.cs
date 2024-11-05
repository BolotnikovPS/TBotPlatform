using System.Text;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.State;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext(
    StateHistory stateHistory,
    IStateBindFactory stateBindFactory,
    ITelegramMappingHandler telegramMapping,
    ITelegramContext telegramContext,
    long chatId
    ) : IStateContext
{
    public StateResult StateResult { get; set; }
    public MarkupNextState MarkupNextState { get; private set; }
    public ChatUpdate ChatUpdate { get; private set; }
    
    private long ChatId { get; set; } = chatId;

    private const string ChooseAction = "😊 Выберите действие";

    public void CreateStateContext(ChatUpdate chatUpdate, MarkupNextState markupNextState)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        StateResult = new();
        MarkupNextState = markupNextState;
        ChatUpdate = chatUpdate;
    }

    public async Task<T> MakeRequestToOtherChatAsync<T>(long newChatId, Func<IStateContextMinimal, Task<T>> request)
    {
        if (newChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (ChatId == newChatId)
        {
            return await request.Invoke(this);
        }

        var baseChatId = ChatId;
        try
        {
            ChatId = newChatId;
            return await request.Invoke(this);
        }
        finally
        {
            ChatId = baseChatId;
        }
    }

    public Guid GetCurrentOperation() => telegramContext.GetCurrentOperation();

    public ITelegramContext GetTelegramContext() => telegramContext;

    public Task BindStateAsync(CancellationToken cancellationToken)
        => stateBindFactory.BindStateAsync(ChatId, stateHistory, cancellationToken);

    public Task UnBindStateAsync(CancellationToken cancellationToken)
        => stateBindFactory.UnBindStateAsync(ChatId, cancellationToken);

    public async Task<ChatResult> SendDocumentAsync(FileDataBase documentData, bool disableNotification, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

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
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

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
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        return telegramContext.SendChatActionAsync(ChatId, chatAction, cancellationToken);
    }

    public async Task<ChatResult> RemoveMarkupAsync(string text, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (text.Length > StateContextConstant.TextLength)
        {
            throw new TextLengthException(text.Length, StateContextConstant.TextLength);
        }

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
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (text.Length > StateContextConstant.TextLength)
        {
            throw new TextLengthException(text.Length, StateContextConstant.TextLength);
        }

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

        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (ChatUpdate.CallbackQueryOrNull.IsNull())
        {
            throw new CallbackQueryMessageIdOrNullArgException();
        }

        if (text.Length > StateContextConstant.TextLength)
        {
            throw new TextLengthException(text.Length, StateContextConstant.TextLength);
        }

        var message = new StringBuilder(ChatUpdate.CallbackQueryOrNull!.Text)
                     .AppendLine()
                     .AppendLine(text)
                     .ToString();

        if (message.Length > StateContextConstant.TextLength)
        {
            throw new TextLengthException(message.Length, StateContextConstant.TextLength);
        }

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
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (messageId.IsNull())
        {
            throw new MessageIdArgException();
        }


        return telegramContext.DeleteMessageAsync(ChatId, messageId, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        ChatUpdate = null;
        MarkupNextState = null;
        StateResult = null;

        return ValueTask.CompletedTask;
    }
}