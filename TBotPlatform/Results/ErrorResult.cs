using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TBotPlatform.Results.Enums;

namespace TBotPlatform.Results;

public class ErrorResult(string description, ErrorResultType errorType)
{
    public string Description { get; } = description;

    [JsonConverter(typeof(StringEnumConverter))]
    public ErrorResultType ErrorType { get; } = errorType;

    public static ErrorResult None() => new("", ErrorResultType.None);

    public static ErrorResult Failure(string description) => new(description, ErrorResultType.Failure);

    public static ErrorResult NotFound(string description) => new(description, ErrorResultType.NotFound);

    public static ErrorResult Validation(string description) => new(description, ErrorResultType.Validation);

    public static ErrorResult Conflict(string description) => new(description, ErrorResultType.Conflict);

    public static ErrorResult AccessUnAuthorized(string description) => new(description, ErrorResultType.AccessUnAuthorized);

    public static ErrorResult AccessForbidden(string description) => new(description, ErrorResultType.AccessForbidden);
}
