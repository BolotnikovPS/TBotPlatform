#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatUpdate(
    EChatUpdateType updateType,
    ChatMessage message,
    ChatMessage? editMessageOrNull = null,
    ChatMessage? channelPostOrNull = null,
    ChatMessage? editChannelPostOrNull = null,
    ChatCallbackQuery? callbackQueryOrNull = null,
    ChatMemberUpdate? myMemberUpdateOrNull = null,
    ChatMemberUpdate? memberUpdateOrNull = null,
    ChatJoinRequestQuery? joinRequestOrNull = null,
    ChatInlineQuery? inlineQueryOrNull = null,
    ChatChosenInlineResult? chosenInlineResultOrNull = null,
    ChatPoll? pollOrNull = null,
    ChatPollAnswer? pollAnswerOrNull = null,
    ChatShippingQuery? shippingQueryOrNull = null,
    ChatPreCheckoutQuery? preCheckoutQueryOrNull = null
    )
{
    public EChatUpdateType UpdateType { get; } = updateType;

    /// <summary>
    /// Принимает все сообщения, обычные текстовые, фото или видео, аудио или видео сообщения (кружочки), стикеры, контакты, геопозиция, голосование и так далее.
    /// Всё, что мы отправляем в чат - это все Message
    /// </summary>
    public ChatMessage Message { get; } = message;

    public ChatMessage? EditMessageOrNull { get; } = editMessageOrNull;

    public ChatMessage? ChannelPostOrNull { get; } = channelPostOrNull;

    public ChatMessage? EditChannelPostOrNull { get; } = editChannelPostOrNull;

    /// <summary>
    /// Сообщения с inline кнопками, они висят под сообщением
    /// </summary>
    public ChatCallbackQuery? CallbackQueryOrNull { get; } = callbackQueryOrNull;

    /// <summary>
    /// Действия касающиеся бота в диалоге между пользователем и ботом, изменения в личных сообщениях
    /// </summary>
    public ChatMemberUpdate? MyMemberUpdateOrNull { get; } = myMemberUpdateOrNull;

    /// <summary>
    /// Действия касающиеся людей в чате/канале: зашел, вышел, повысили, понизили и т.д.
    /// </summary>
    public ChatMemberUpdate? MemberUpdateOrNull { get; } = memberUpdateOrNull;

    public ChatJoinRequestQuery? JoinRequestOrNull { get; } = joinRequestOrNull;

    public ChatInlineQuery? InlineQueryOrNull { get; } = inlineQueryOrNull;

    public ChatChosenInlineResult? ChosenInlineResultOrNull { get; } = chosenInlineResultOrNull;

    public ChatPoll? PollOrNull { get; } = pollOrNull;

    public ChatPollAnswer? PollAnswerOrNull { get; } = pollAnswerOrNull;

    public ChatShippingQuery? ShippingQueryOrNull { get; } = shippingQueryOrNull;

    public ChatPreCheckoutQuery? PreCheckoutQueryOrNull { get; } = preCheckoutQueryOrNull;
}