namespace TBotPlatform.Contracts.Abstractions.Cache;

public interface ICacheService
{
    /// <summary>
    /// Добавляет значений в коллекцию
    /// </summary>
    /// <param name="collection">Название коллекции</param>
    /// <param name="value">Значение</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddValueToCollectionAsync(string collection, IKeyInCache value, CancellationToken cancellationToken);

    /// <summary>
    /// Получает значение с коллекции
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">Название коллекции</param>
    /// <param name="key">Ключ</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> GetValueFromCollectionAsync<T>(string collection, string key, CancellationToken cancellationToken)
        where T : IKeyInCache;

    /// <summary>
    /// Получает все значения с коллекции
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">Название коллекции</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<T>> GetAllValueFromCollectionAsync<T>(string collection, CancellationToken cancellationToken)
        where T : IKeyInCache;

    /// <summary>
    /// Удаляет значение с коллекции
    /// </summary>
    /// <param name="collection">Название коллекции</param>
    /// <param name="key">Ключ</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveValueFromCollectionAsync(string collection, string key, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет коллекции
    /// </summary>
    /// <param name="collection">Название коллекции</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveCollectionAsync(string collection, CancellationToken cancellationToken);

    /// <summary>
    /// Получает значение
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Ключ</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> GetValueAsync<T>(string key, CancellationToken cancellationToken)
        where T : IKeyInCache;

    /// <summary>
    /// Добавляет значение с временем жизни
    /// </summary>
    /// <param name="value">Значение</param>
    /// <param name="expiryTime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetValueAsync(IKeyInCache value, TimeSpan expiryTime, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет значение
    /// </summary>
    /// <param name="value">Значение</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetValueAsync(IKeyInCache value, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет значение
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> RemoveValueAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Проверяет наличие ключа
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> KeyExistsAsync(string key, CancellationToken cancellationToken);
}