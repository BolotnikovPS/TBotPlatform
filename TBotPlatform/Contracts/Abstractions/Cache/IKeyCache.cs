namespace TBotPlatform.Contracts.Abstractions.Cache;

public interface IKeyInCache
{
    /// <summary>
    /// Key to look up in the cache
    /// </summary>
    string Key { get; }
}