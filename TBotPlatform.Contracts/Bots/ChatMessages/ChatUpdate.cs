#nullable enable
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Contracts.Bots.ChatMessages;

public class ChatUpdate(
    UpdateType updateType,
    ChatUpdateMessage message,
    ChatUpdateForwardFromUser? forwardFromUserOrNull,
    ChatUpdateCallbackQuery? callbackQueryOrNull,
    ChatUpdateMemberUpdate? myChatMessageMemberUpdate,
    ChatUpdateMemberUpdate? chatMessageMemberUpdate
    )
{
    public UpdateType UpdateType { get; } = updateType;

    /// <summary>
    /// Принимает все сообщения, обычные текстовые, фото или видео, аудио или видео сообщения (кружочки), стикеры, контакты, геопозиция, голосование и так далее.
    /// Всё, что мы отправляем в чат - это все Message
    /// </summary>
    public ChatUpdateMessage Message { get; } = message;

    /// <summary>
    /// Сообщения с inline кнопками, они висят под сообщением
    /// </summary>
    public ChatUpdateCallbackQuery? CallbackQueryOrNull { get; } = callbackQueryOrNull;
    
    /// <summary>
    /// Сообщение пересланное от пользователя
    /// </summary>
    public ChatUpdateForwardFromUser? ForwardFromUserOrNull { get; } = forwardFromUserOrNull;

    /// <summary>
    /// Действия касающиеся бота в диалоге между пользователем и ботом, изменения в личных сообщениях
    /// </summary>
    public ChatUpdateMemberUpdate? MyChatMemberUpdateOrNull { get; } = myChatMessageMemberUpdate;

    /// <summary>
    /// Действия касающиеся людей в чате/канале: зашел, вышел, повысили, понизили и т.д.
    /// </summary>
    public ChatUpdateMemberUpdate? ChatMemberUpdateOrNull { get; } = chatMessageMemberUpdate;
}