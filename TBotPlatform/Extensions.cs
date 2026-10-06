using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;

namespace TBotPlatform.Common;

public static partial class Extensions
{
    /// <summary>
    /// Sets the flag telling whether the main buttons must be updated
    /// </summary>
    /// <param name="context">Context used to process the message</param>
    public static void SetNeedUpdateMarkup(this IStateContext context) => context.StateResult.IsNeedUpdateMarkup = true;

    /// <summary>
    /// Sets the name of the next state
    /// </summary>
    /// <param name="context">Context used to process the message</param>
    /// <param name="nextStateName">State name</param>
    public static void SetNextStateName(this IStateContext context, string nextStateName) => context.StateResult.NextStateName = nextStateName;

    /// <summary>
    /// Sets the callback data
    /// </summary>
    /// <param name="context">Context used to process the message</param>
    /// <param name="data">Data</param>
    public static void SetData(this IStateContext context, string data) => context.StateResult.Data = data;

    /// <summary>
    /// Splits a string into chunks of the given length
    /// </summary>
    /// <param name="str">String</param>
    /// <param name="maxLength">Maximum string length</param>
    public static IEnumerable<string> SplitByLength(this string str, int maxLength)
    {
        for (var index = 0; index < str.Length; index += maxLength)
        {
            yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
        }
    }
}