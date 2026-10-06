using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.Markups.InlineMarkups;
using TBotPlatform.Extension;
using Telegram.Bot.Types;

namespace TBotPlatform.Samples.EchoBot;

/// <summary>
/// Demonstrates paginated output.
/// Page numbers come from the inline buttons <see cref="PaginationsConstant.PreviousPage"/> and
/// <see cref="PaginationsConstant.NextPage"/>: <see cref="InlineMarkupState"/> itself adds the prefix
/// <see cref="PaginationsConstant.PaginationIdentity"/> to the data of that button, and
/// <c>TryParsePagination</c> parses it back.
/// </summary>
internal static class CityList
{
    /// <summary>
    /// Number of items per page
    /// </summary>
    private const int Step = PaginationsConstant.StepMin;

    /// <summary>
    /// State class that handles next/previous page button presses
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
    /// Total page count
    /// </summary>
    public static int PagesCount => (Cities.Count + Step - 1) / Step;

    /// <summary>
    /// Sends or updates the message with the list page
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
