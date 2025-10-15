#nullable enable
using TBotPlatform.Results.Abstractions;

namespace TBotPlatform.Results;

public sealed class ResultT<TValue> : Result, IResult<TValue>
{
    private readonly TValue? _value;

    private ResultT(TValue value) : base() => _value = value;

    private ResultT(ErrorResult error) : base(error) => _value = default;

    public TValue Value
        => IsSuccess ? _value! : throw new InvalidOperationException("Value can not be accessed when IsSuccess is false");

    public static implicit operator ResultT<TValue>(ErrorResult error) => new(error);

    public static implicit operator ResultT<TValue>(TValue value) => new(value);

    public static ResultT<TValue> Success(TValue value) => new(value);

    public static new ResultT<TValue> Failure(ErrorResult error) => new(error);
}