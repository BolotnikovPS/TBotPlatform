#nullable enable
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public string? GetMessageLink()
        => GetChatUpdateMessage()?.MessageLink();

    private Message? GetChatUpdateMessage()
        => ChatUpdate switch
        {
            { Message: { } message } => message,
            { EditedMessage: { } editedMessage } => editedMessage,
            { ChannelPost: { } channelPost } => channelPost,
            { EditedChannelPost: { } editedChannelPost } => editedChannelPost,
            { BusinessMessage: { } businessMessage } => businessMessage,
            { EditedBusinessMessage: { } editedBusinessMessage } => editedBusinessMessage,
            { GuestMessage: { } guestMessage } => guestMessage,
            { CallbackQuery.Message: { } callbackMessage } => callbackMessage,
            _ => null,
        };
}
