using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.Markups.InlineMarkups;
using TBotPlatform.Extension;
using Telegram.Bot.Types;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Демонстрация постраничного вывода (пагинации).
/// Номера страниц приходят с inline кнопок <see cref="PaginationsConstant.PreviousPage"/> и
/// <see cref="PaginationsConstant.NextPage"/>: <see cref="InlineMarkupState"/> сам добавляет к данным
/// этой кнопки префикс <see cref="PaginationsConstant.PaginationIdentity"/>, а
/// <c>TryParsePagination</c> разбирает его обратно.
/// </summary>
internal static class CityList
{
    /// <summary>
    /// Число элементов на странице
    /// </summary>
    private const int Step = PaginationsConstant.StepMin;

    /// <summary>
    /// Класс состояния, которое обрабатывает нажатия на кнопки перехода по страницам
    /// </summary>
    private const string PageStateName = nameof(ListPageState);

    private static readonly List<string> Cities =
    [
        "Москва",
        "Санкт-Петербург",
        "Новосибирск",
        "Екатеринбург",
        "Казань",
        "Нижний Новгород",
        "Челябинск",
        "Самара",
        "Омск",
        "Ростов-на-Дону",
        "Уфа",
        "Красноярск",
        "Воронеж",
        "Пермь",
        "Волгоград",
        "Краснодар",
        "Саратов",
        "Тюмень",
        "Тольятти",
        "Ижевск",
    ];

    /// <summary>
    /// Общее число страниц
    /// </summary>
    public static int PagesCount => (Cities.Count + Step - 1) / Step;

    /// <summary>
    /// Отправляет или обновляет сообщение со страницей списка
    /// </summary>
    public static Task<Message> RenderPage(IStateContext context, int page, CancellationToken cancellationToken)
    {
        var pagination = Cities.GetPaginationData(Step, page);
        var currentPage = Math.Clamp(page, 1, PagesCount);

        var buttons = new InlineMarkupList();

        if (pagination.IsPrevious)
        {
            buttons.Add(new InlineMarkupState(PaginationsConstant.PreviousPage, PageStateName, pagination.PreviousValue));
        }

        if (pagination.IsNext)
        {
            buttons.Add(new InlineMarkupState(PaginationsConstant.NextPage, PageStateName, pagination.NextValue));
        }

        var markup = new InlineMarkupMassiveList();

        if (buttons.CheckAny())
        {
            markup.Add(
                new InlineMarkupMassive
                {
                    InlineMarkups = buttons,
                    ButtonsPerRow = 2,
                });
        }

        var text = $"Города, страница {currentPage} из {PagesCount}:"
                   + Environment.NewLine
                   + Environment.NewLine
                   + string.Join(Environment.NewLine, pagination.Values);

        return context.SendOrUpdateTextMessage(text, markup, cancellationToken);
    }
}
