namespace TBotPlatform.Contracts.Abstractions.Cache;

public interface IKeyInCache
{
    /// <summary>
    /// Ключ для поиска в кэше
    /// </summary>
    string Key { get; }
}