﻿#nullable enable
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;

namespace TBotPlatform.Contracts.Abstractions.Handlers;

public interface IStartReceivingHandler
{
    /// <summary>
    /// Обрабатывает данные при поступлении сообщения
    /// </summary>
    /// <param name="chatUpdate">Форматированное сообщение с telegram</param>
    /// <param name="markupNextState">Данные с inline кнопки</param>
    /// <param name="telegramUser">Пользователь telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleUpdateAsync(ChatUpdate chatUpdate, MarkupNextState? markupNextState, TelegramMessageUserData telegramUser, CancellationToken cancellationToken);
}