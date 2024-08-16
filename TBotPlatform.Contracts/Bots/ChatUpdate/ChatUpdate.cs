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

    /// <summary>
    /// Смотреть описание Message
    /// </summary>
    public ChatMessage? EditMessageOrNull { get; } = editMessageOrNull;

    /// <summary>
    /// Смотреть описание Message. Направлен на работу с каналами
    /// </summary>
    public ChatMessage? ChannelPostOrNull { get; } = channelPostOrNull;

    /// <summary>
    /// Смотреть описание Message. Направлен на работу с каналами
    /// </summary>
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

    /// <summary>
    /// Получение информации о поданной заявки на вступление в чат/канал.
    /// </summary>
    public ChatJoinRequestQuery? JoinRequestOrNull { get; } = joinRequestOrNull;

    /// <summary>
    /// Получение входящих Inline запросов.
    /// Inline запрос - отправка сообщения в чат используя "@username" бота и вводя какой-то запрос
    /// </summary>
    public ChatInlineQuery? InlineQueryOrNull { get; } = inlineQueryOrNull;

    /// <summary>
    /// Обработка Inline запроса. Получает выбор пользователя и обрабатывает его.
    /// </summary>
    public ChatChosenInlineResult? ChosenInlineResultOrNull { get; } = chosenInlineResultOrNull;

    /// <summary>
    /// Все связанное с голосованием
    /// </summary>
    public ChatPoll? PollOrNull { get; } = pollOrNull;

    /// <summary>
    /// Когда пользователь выбрал ответ в голосовании
    /// </summary>
    public ChatPollAnswer? PollAnswerOrNull { get; } = pollAnswerOrNull;

    /// <summary>
    /// Информация о платеже, который начал оплачивать пользователь
    /// </summary>
    public ChatShippingQuery? ShippingQueryOrNull { get; } = shippingQueryOrNull;

    /// <summary>
    /// Подтверждение успешной оплаты
    /// </summary>
    public ChatPreCheckoutQuery? PreCheckoutQueryOrNull { get; } = preCheckoutQueryOrNull;
}