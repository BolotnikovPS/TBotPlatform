using TBotPlatform.Contracts.Abstractions.Cache;

namespace TBotPlatform.Contracts.Cache;

public class BaseStateInCache<T> : IKeyInCache
{
    public T Value { get; set; } = default!;
    public string Key { get; set; } = null!;
}