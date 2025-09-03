#nullable enable
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Bots.States;
using TBotPlatform.Extension;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using PMode = Telegram.Bot.Types.Enums.ParseMode;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext(
    AsyncServiceScope scope,
    StateHistory? stateHistory,
    IStateBindFactory stateBindFactory,
    ITelegramContext telegramContext,
    long chatId
    ) : IStateContext
{
    public StateResult StateResult { get; set; } = new();
    public MarkupNextState? MarkupNextState { get; private set; }
    public Update? ChatUpdate { get; private set; }

    internal long ChatId { get; set; } = chatId;

    private const PMode ParseMode = PMode.Html;
    private const string ChooseAction = "😊 Выберите действие";

    public void CreateStateContext(Update chatUpdate, MarkupNextState? markupNextState)
    {
        ChatIdValidOrThrow();

        MarkupNextState = markupNextState;
        ChatUpdate = chatUpdate;
    }

    public Guid CurrentOperation => telegramContext.CurrentOperation;

    public ITelegramContext TelegramContext => telegramContext;

    public string BotName => telegramContext.GetTelegramSettings().BotName;

    public async Task<T> MakeRequestToOtherChat<T>(long newChatId, Func<IStateContextMinimal, Task<T>> request)
    {
        ChatIdValidOrThrow(newChatId);

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

    public Task BindState(CancellationToken cancellationToken)
    {
        if (stateHistory.IsNull())
        {
            throw new NullReferenceException("История состояния отсутствует.");
        }

        return stateBindFactory.BindState(telegramContext.GetTelegramSettings().BotName, ChatId, stateHistory, cancellationToken);
    }

    public Task UnBindState(CancellationToken cancellationToken)
        => stateBindFactory.UnBindState(telegramContext.GetTelegramSettings().BotName, ChatId, cancellationToken);

    public async Task<Message> SendDocument(FileDataBase documentData, string? caption, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        await using var fileStream = new MemoryStream(documentData.Bytes);

        var inputFile = InputFile.FromStream(fileStream, documentData.Name);

        return await telegramContext.SendDocument(
            ChatId,
            inputFile,
            caption,
            ParseMode,
            replyParameters: null,
            disableNotification: disableNotification,
            cancellationToken: cancellationToken);
    }

    public Task<Message> SendDocument(FileDataBase documentData, bool disableNotification, CancellationToken cancellationToken)
        => SendDocument(documentData, caption: null, disableNotification, cancellationToken);

    public Task<Message> SendDocument(FileDataBase documentData, CancellationToken cancellationToken)
        => SendDocument(documentData, disableNotification: false, cancellationToken);

    public async Task<Message> SendPhoto(FileDataBase documentData, string? caption, bool disableNotification, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        await using var fileStream = new MemoryStream(documentData.Bytes);

        var inputFile = InputFile.FromStream(fileStream, documentData.Name);

        return await telegramContext.SendPhoto(
            ChatId,
            inputFile,
            caption,
            ParseMode,
            replyParameters: null,
            disableNotification: disableNotification,
            cancellationToken: cancellationToken
            );
    }

    public Task<Message> SendPhoto(FileDataBase documentData, bool disableNotification, CancellationToken cancellationToken)
        => SendPhoto(documentData, caption: null, disableNotification: false, cancellationToken);

    public Task<Message> SendPhoto(FileDataBase documentData, CancellationToken cancellationToken)
        => SendPhoto(documentData, disableNotification: false, cancellationToken);

    public Task SendChatAction(ChatAction chatAction, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();

        return telegramContext.SendChatAction(ChatId, chatAction, cancellationToken: cancellationToken);
    }

    public Task<Message> RemoveMarkup(string text, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        TextLengthValidOrThrow(text);

        return telegramContext.SendMessage(
            ChatId,
            text,
            ParseMode,
            replyParameters: null,
            new ReplyKeyboardRemove(),
            disableNotification: false,
            cancellationToken: cancellationToken
            );
    }

    public Task<Message> UpdateMainButtons(MainButtonMassiveList mainButtonMassiveList, CancellationToken cancellationToken)
        => UpdateMainButtons(mainButtonMassiveList, ChooseAction, cancellationToken);

    public Task<Message> UpdateMainButtons(MainButtonMassiveList mainButtonMassiveList, string text, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        TextLengthValidOrThrow(text);

        var newMarkup = Map(mainButtonMassiveList);

        if (newMarkup.IsNull())
        {
            throw new ReplyKeyboardMarkupArgException();
        }

        return telegramContext.SendMessage(
            ChatId,
            text,
            ParseMode,
            replyParameters: null,
            newMarkup,
            disableNotification: false,
            cancellationToken: cancellationToken
            );
    }

    public Task UpdateMarkupTextAndDropButton(string text, CancellationToken cancellationToken)
    {
        if (MarkupNextState?.State == null)
        {
            return Task.CompletedTask;
        }

        ChatIdValidOrThrow();
        CallbackQueryValidOrThrow(ChatUpdate!.CallbackQuery);
        TextLengthValidOrThrow(text);

        var message = new StringBuilder(ChatUpdate.CallbackQuery!.Message?.Text)
                     .AppendLine()
                     .AppendLine(text)
                     .ToString();

        TextLengthValidOrThrow(message);

        if (ChatUpdate.CallbackQuery.WithPhoto())
        {
            return telegramContext.EditMessageCaption(
                ChatId,
                ChatUpdate.CallbackQuery!.Message!.MessageId,
                message,
                ParseMode,
                cancellationToken: cancellationToken
                );
        }

        return telegramContext.EditMessageText(
            ChatId,
            ChatUpdate.CallbackQuery!.Message!.MessageId,
            message,
            ParseMode,
            cancellationToken: cancellationToken
            );
    }

    public Task UpdateMarkupTextAndDropButton(CancellationToken cancellationToken)
        => UpdateMarkupTextAndDropButton("", cancellationToken);

    public Task RemoveMessage(int messageId, CancellationToken cancellationToken)
    {
        ChatIdValidOrThrow();
        MessageIdValidOrThrow(messageId);

        return telegramContext.DeleteMessage(ChatId, messageId, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        ChatUpdate = null;
        MarkupNextState = null;

        await scope.DisposeAsync();
    }
}