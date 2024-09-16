using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

namespace TBotPlatform.Contracts;

public static class Extensions
{
    public static void SetNeedUpdateMarkup(this IStateContext context) => context.StateResult.IsNeedUpdateMarkup = true;
}