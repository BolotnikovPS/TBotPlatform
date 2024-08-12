#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatUpdate(
    EChatUpdateType updateType,
    ChatUpdateMessage message,
    ChatUpdateMessage? editMessageOrNull = null,
    ChatUpdateMessage? channelPostOrNull = null,
    ChatUpdateMessage? editChannelPostOrNull = null,
    ChatUpdateCallbackQuery? callbackQueryOrNull = null,
    ChatUpdateMemberUpdate? myMemberUpdateOrNull = null,
    ChatUpdateMemberUpdate? memberUpdateOrNull = null,
    ChatUpdateJoinRequest? joinRequestOrNull = null,
    ChatUpdateInlineQuery? inlineQueryOrNull = null,
    ChatUpdateChosenInlineResult? chosenInlineResultOrNull = null,
    ChatUpdatePoll? pollOrNull = null,
    ChatUpdatePollAnswer? pollAnswerOrNull = null,
    ChatUpdateShippingQuery? shippingQueryOrNull = null,
    ChatUpdatePreCheckoutQuery? preCheckoutQueryOrNull = null
    )
{
    public EChatUpdateType UpdateType { get; } = updateType;

    /// <summary>
    /// Принимает все сообщения, обычные текстовые, фото или видео, аудио или видео сообщения (кружочки), стикеры, контакты, геопозиция, голосование и так далее.
    /// Всё, что мы отправляем в чат - это все Message
    /// </summary>
    public ChatUpdateMessage Message { get; } = message;

    public ChatUpdateMessage? EditMessageOrNull { get; } = editMessageOrNull;

    public ChatUpdateMessage? ChannelPostOrNull { get; } = channelPostOrNull;

    public ChatUpdateMessage? EditChannelPostOrNull { get; } = editChannelPostOrNull;

    /// <summary>
    /// Сообщения с inline кнопками, они висят под сообщением
    /// </summary>
    public ChatUpdateCallbackQuery? CallbackQueryOrNull { get; } = callbackQueryOrNull;

    /// <summary>
    /// Действия касающиеся бота в диалоге между пользователем и ботом, изменения в личных сообщениях
    /// </summary>
    public ChatUpdateMemberUpdate? MyMemberUpdateOrNull { get; } = myMemberUpdateOrNull;

    /// <summary>
    /// Действия касающиеся людей в чате/канале: зашел, вышел, повысили, понизили и т.д.
    /// </summary>
    public ChatUpdateMemberUpdate? MemberUpdateOrNull { get; } = memberUpdateOrNull;

    public ChatUpdateJoinRequest? JoinRequestOrNull { get; } = joinRequestOrNull;

    public ChatUpdateInlineQuery? InlineQueryOrNull { get; } = inlineQueryOrNull;

    public ChatUpdateChosenInlineResult? ChosenInlineResultOrNull { get; } = chosenInlineResultOrNull;

    public ChatUpdatePoll? PollOrNull { get; } = pollOrNull;

    public ChatUpdatePollAnswer? PollAnswerOrNull { get; } = pollAnswerOrNull;

    public ChatUpdateShippingQuery? ShippingQueryOrNull { get; } = shippingQueryOrNull;

    public ChatUpdatePreCheckoutQuery? PreCheckoutQueryOrNull { get; } = preCheckoutQueryOrNull;
}