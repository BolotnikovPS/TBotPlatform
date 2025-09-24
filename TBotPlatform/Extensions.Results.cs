using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform;

public static partial class Extensions
{
    public static T Match<T>(this IResult result, Func<T> onSuccess, Func<ErrorResult, T> onFailure) => result.IsSuccess ? onSuccess() : onFailure(result.Error!);

    public static T Match<T, TValue>(this IResult<TValue> result, Func<TValue, T> onSuccess, Func<ErrorResult, T> onFailure) => result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error!);
}