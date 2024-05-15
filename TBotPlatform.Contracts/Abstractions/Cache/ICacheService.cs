namespace TBotPlatform.Contracts.Abstractions.Cache;

public interface ICacheService
{
    Task AddValueToCollectionAsync(string collection, IKeyInCache value, CancellationToken cancellationToken);

    Task<T> GetValueFromCollectionAsync<T>(string collection, string key, CancellationToken cancellationToken)
        where T : IKeyInCache;

    Task<List<T>> GetAllValueFromCollectionAsync<T>(string collection, CancellationToken cancellationToken)
        where T : IKeyInCache;

    Task RemoveValueFromCollectionAsync(string collection, string key, CancellationToken cancellationToken);

    Task RemoveCollectionAsync(string collection, CancellationToken cancellationToken);

    Task<T> GetValueAsync<T>(string key, CancellationToken cancellationToken)
        where T : IKeyInCache;

    Task<bool> SetValueAsync(IKeyInCache value, TimeSpan expiryTime, CancellationToken cancellationToken);

    Task<bool> SetValueAsync(IKeyInCache value, CancellationToken cancellationToken);

    Task<bool> RemoveValueAsync(string key, CancellationToken cancellationToken);
}