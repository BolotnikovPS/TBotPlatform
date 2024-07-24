namespace TBotPlatform.Contracts.Abstractions.Cache;

public interface IKeyInCache
{
    /// <summary>
    /// Ключ для поиска в кеше
    /// </summary>
    string Key { get; }
}