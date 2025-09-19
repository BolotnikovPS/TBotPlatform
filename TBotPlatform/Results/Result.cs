#nullable enable
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Results;

public class Result : IResult
{
    protected Result()
    {
        IsSuccess = true;
        Error = ErrorResult.None();
    }

    protected Result(ErrorResult error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ErrorResult? Error { get; }

    public static implicit operator Result(ErrorResult error) => new(error);

    public static Result Success() => new();

    public static Result Failure(ErrorResult error) => new(error);
}