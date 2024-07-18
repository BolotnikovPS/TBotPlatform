using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots.Pagination;

public static class PaginationExtensions
{
    public static bool TryParsePagination(this MarkupNextState markupNextState, out int result)
    {
        result = 0;

        if (markupNextState.IsNull()
            || markupNextState.Data.IsNull()
           )
        {
            return false;
        }

        return markupNextState.Data.Contains(PaginationsConstant.PaginationIdentity)
               && int.TryParse(markupNextState.Data.Replace(PaginationsConstant.PaginationIdentity, ""), out result);
    }

    public static PaginationData<T> GetPaginationData<T>(this List<T> values, int step, int currentPosition)
        where T : class
    {
        if (currentPosition == 0)
        {
            currentPosition = 1;
        }

        var skip = (currentPosition - 1) * step;
        var data = values
                  .Skip(skip)
                  .Take(step)
                  .ToList();

        var isNext = skip + step < values.Count;
        var isPrevious = currentPosition != 1;

        return new()
        {
            Values = data,
            IsNext = isNext,
            IsPrevious = isPrevious,
            NextValue = (currentPosition + 1).ToString(),
            PreviousValue = (currentPosition - 1).ToString(),
        };
    }
}