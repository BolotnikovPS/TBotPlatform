#nullable enable
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.ChatUpdate;
using TBotPlatform.Results.Abstractions;
using Telegram.Bot.Types;

namespace TBotPlatform.Contracts.Abstractions.Handlers;

public interface IStartReceivingHandler
{
    /// <summary>
    /// Обрабатывает данные при поступлении сообщения
    /// </summary>
    /// <param name="botName">Наименование бота</param>
    /// <param name="update">Запрос с telegram</param>
    /// <param name="markupNextState">Данные с inline кнопки</param>
    /// <param name="telegramData">Данные о пользователе и чате telegram</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult> HandleUpdate(string botName, Update update, MarkupNextState? markupNextState, TelegramMessageUserData telegramData, CancellationToken cancellationToken);
}