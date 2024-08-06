using System.Text;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.ChatMessages;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.StateContext;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Contexts;

internal partial class StateContext(ITelegramContext botClient, long chatId) : IStateContext
{
    public MarkupNextState MarkupNextState { get; private set; }
    public ChatMessage ChatMessage { get; private set; }
    public EBindStateType BindState { get; private set; }
    public bool IsForceReplyLastMenu { get; private set; }

    private long ChatId { get; } = chatId;

    private const string ChooseAction = "😊 Выберите действие";

    public void CreateStateContext(ChatMessage chatMessage, MarkupNextState markupNextState)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        ChatMessage = chatMessage;
        MarkupNextState = markupNextState;
    }

    public Guid GetCurrentOperation() => botClient.GetCurrentOperation();

    public void SetNeedIsForceReplyLastMenu() => IsForceReplyLastMenu = true;

    public void SetBindState(EBindStateType type) => BindState = type;

    public Task<Message> SendDocumentAsync(InputFile inputFile, bool disableNotification, CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        return botClient.SendDocumentAsync(
            ChatId,
            inputFile,
            disableNotification,
            cancellationToken
            );
    }

    public Task<Message> SendDocumentAsync(InputFile inputFile, CancellationToken cancellationToken)
        => SendDocumentAsync(inputFile, false, cancellationToken);

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

    public Task<Message> UpdateMarkupAsync(ButtonsRuleMassiveList replyMarkup, CancellationToken cancellationToken)
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

        return botClient.SendTextMessageAsync(
            ChatId,
            ChooseAction,
            newMarkup,
            false,
            cancellationToken
            );
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

        if (ChatMessage.CallbackQueryOrNull.IsNull())
        {
            throw new CallbackQueryMessageIdOrNullArgException();
        }

        if (text.Length > StateContextConstant.TextLength)
        {
            throw new TextLengthException(text.Length, StateContextConstant.TextLength);
        }

        var message = new StringBuilder(ChatMessage.CallbackQueryOrNull!.CallbackQueryMessage)
                     .AppendLine()
                     .AppendLine(text)
                     .ToString();

        if (message.Length > StateContextConstant.TextLength)
        {
            throw new TextLengthException(message.Length, StateContextConstant.TextLength);
        }

        if (ChatMessage.CallbackQueryOrNull!.CallbackQueryMessageWithImage)
        {
            return botClient.EditMessageCaptionAsync(
                ChatId,
                ChatMessage.CallbackQueryOrNull!.CallbackQueryMessageId,
                message,
                cancellationToken
                );
        }

        return botClient.EditMessageTextAsync(
            ChatId,
            ChatMessage.CallbackQueryOrNull!.CallbackQueryMessageId,
            message,
            cancellationToken
            );
    }

    public Task UpdateMarkupTextAndDropButtonAsync(CancellationToken cancellationToken)
        => UpdateMarkupTextAndDropButtonAsync("", cancellationToken);

    public Task RemoveCurrentReplyMessageAsync(CancellationToken cancellationToken)
    {
        if (ChatId.IsDefault())
        {
            throw new ChatIdArgException();
        }

        if (ChatMessage.CallbackQueryOrNull.IsNull())
        {
            throw new CallbackQueryMessageIdOrNullArgException();
        }

        return botClient.DeleteMessageAsync(
            ChatId,
            ChatMessage.CallbackQueryOrNull!.CallbackQueryMessageId,
            cancellationToken
            );
    }

    public ValueTask DisposeAsync()
    {
        CleanUp();

        return ValueTask.CompletedTask;
    }

    private void CleanUp()
    {
        ChatMessage = null;
        MarkupNextState = null;
        IsForceReplyLastMenu = false;
    }
}