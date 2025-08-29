#nullable enable
using TBotPlatform.Common.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.State;

namespace TBotPlatform.Common;

public static partial class Extensions
{
    /// <summary>
    /// Получает результат выполнения состояния
    /// </summary>
    /// <param name="stateContext">Контекст состояния</param>
    /// <param name="result">Возвращаемое значение</param>
    /// <returns></returns>
    public static bool TryGetStateResult(this IStateContextMinimal stateContext, out StateResult? result)
    {
        if (stateContext is BaseStateContext context)
        {
            result = context.StateResult;
            return true;
        }

        result = null;
        return false;
    }
}