using System.Text;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatResults;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.StateContext;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext(ITelegramMappingHandler telegramMapping, ITelegramContext botClient, long chatId) : IStateContext
{
    public MarkupNextState MarkupNextState { get; private set; }
    public ChatUpdate ChatUpdate { get; private set; }
    public EBindStateType BindState { get; private set; }
    public bool IsForceReplyLastMenu { get; private set; }

    private long ChatId { get; } = chatId;

    private const string ChooseAction = "😊 Выберите действие";

    public void CreateStateContext(ChatUpdate chatUpdate, MarkupNextState markupNextState)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        ChatUpdate = chatUpdate;
        MarkupNextState = markupNextState;
    }

    public Guid GetCurrentOperation() => botClient.GetCurrentOperation();

    public void SetNeedIsForceReplyLastMenu() => IsForceReplyLastMenu = true;

    public void SetBindState(EBindStateType type) => BindState = type;

    public async Task<ChatResult> SendDocumentAsync(byte[] fileBytes, string fileName, bool disableNotification, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        await using var fileStream = new MemoryStream(fileBytes);

        var inputFile = InputFile.FromStream(fileStream, fileName);

        var result = await botClient.SendDocumentAsync(
            ChatId,
            inputFile,
            disableNotification,
            cancellationToken
            );

        return telegramMapping.MessageToResult(result);
    }

    public Task<ChatResult> SendDocumentAsync(byte[] fileBytes, string fileName, CancellationToken cancellationToken)
        => SendDocumentAsync(fileBytes, fileName, false, cancellationToken);

    public Task SendChatActionAsync(ChatAction chatAction, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        return botClient.SendChatActionAsync(
            ChatId,
            chatAction,
            cancellationToken
            );
    }

    public async Task<ChatResult> UpdateMarkupAsync(ButtonsRuleMassiveList replyMarkup, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        var newMarkup = Map(replyMarkup);

        if (newMarkup.IsNull())
        {
            throw new ReplyKeyboardMarkupArgException();
        }

        var result = await botClient.SendTextMessageAsync(
            ChatId,
            ChooseAction,
            newMarkup,
            false,
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
            return botClient.EditMessageCaptionAsync(
                ChatId,
                ChatUpdate.CallbackQueryOrNull!.MessageId,
                message,
                cancellationToken
                );
        }

        return botClient.EditMessageTextAsync(
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


        return botClient.DeleteMessageAsync(ChatId, messageId, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        CleanUp();

        return ValueTask.CompletedTask;
    }

    private void CleanUp()
    {
        ChatUpdate = null;
        MarkupNextState = null;
        IsForceReplyLastMenu = false;
    }
}