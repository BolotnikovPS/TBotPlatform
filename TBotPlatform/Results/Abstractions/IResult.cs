#nullable enable
namespace TBotPlatform.Results.Abstractions;

public interface IResult
{
    bool IsSuccess { get; }

    public ErrorResult? Error { get; }
}

public interface IResult<out TResponse> : IResult
{
    TResponse Value { get; }
}
