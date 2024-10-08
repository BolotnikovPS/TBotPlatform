﻿#nullable enable
using TBotPlatform.Contracts.Bots.Chats;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatJoinRequestQuery(TelegramChat? chatOrNull, TelegramUser fromUser, long userChatId, DateTime date)
{
    public TelegramChat? ChatOrNull { get; } = chatOrNull;

    /// <summary>
    /// Данные пользователя
    /// </summary>
    public TelegramUser FromUser { get; } = fromUser;

    /// <summary>
    /// Id чата
    /// </summary>
    public long UserChatId { get; } = userChatId;

    /// <summary>
    /// Дата запроса
    /// </summary>
    public DateTime Date { get; } = date;
}