using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

namespace TBotPlatform.Contracts;

public static partial class Extensions
{
    public static void SetNeedUpdateMarkup(this IStateContext context) => context.StateResult.IsNeedUpdateMarkup = true;

    public static void SetNextStateName(this IStateContext context, string nextStateName) => context.StateResult.NextStateName = nextStateName;

    public static void SetData(this IStateContext context, string data) => context.StateResult.Data = data;
}