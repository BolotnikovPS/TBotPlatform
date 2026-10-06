#nullable enable
using TBotPlatform.Common.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots.States;

namespace TBotPlatform.Common;

public static partial class Extensions
{
    /// <summary>
    /// Gets the result of the state execution
    /// </summary>
    /// <param name="stateContext">State context</param>
    /// <param name="result">Return value</param>
    public static bool TryGetStateResult(this IStateContextMinimal stateContext, out StateResult? result)
    {
        if (stateContext is StateContext context)
        {
            result = context.StateResult;
            return true;
        }

        result = null;
        return false;
    }
}