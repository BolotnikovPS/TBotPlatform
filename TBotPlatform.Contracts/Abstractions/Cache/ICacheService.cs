namespace TBotPlatform.Contracts.Abstractions.Cache;

public interface ICacheService
{
    /// <summary>
    /// Добавляет значений в коллекцию
    /// </summary>
    /// <param name="collection">Название коллекции</param>
    /// <param name="value">Значение</param>
    /// <returns></returns>
    Task AddValueToCollection(string collection, IKeyInCache value);

    /// <summary>
    /// Получает значение с коллекции
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">Название коллекции</param>
    /// <param name="key">Ключ</param>
    /// <returns></returns>
    Task<T> GetValueFromCollection<T>(string collection, string key)
        where T : IKeyInCache;

    /// <summary>
    /// Получает все значения с коллекции
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">Название коллекции</param>
    /// <returns></returns>
    Task<List<T>> GetAllValueFromCollection<T>(string collection)
        where T : IKeyInCache;

    /// <summary>
    /// Удаляет значение с коллекции
    /// </summary>
    /// <param name="collection">Название коллекции</param>
    /// <param name="key">Ключ</param>
    /// <returns></returns>
    Task RemoveValueFromCollection(string collection, string key);

    /// <summary>
    /// Удаляет коллекции
    /// </summary>
    /// <param name="collection">Название коллекции</param>
    /// <returns></returns>
    Task RemoveCollection(string collection);

    /// <summary>
    /// Получает значение
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Ключ</param>
    /// <returns></returns>
    Task<T> GetValue<T>(string key)
        where T : IKeyInCache;

    /// <summary>
    /// Добавляет значение с временем жизни
    /// </summary>
    /// <param name="value">Значение</param>
    /// <param name="expiryTime"></param>
    /// <returns></returns>
    Task<bool> SetValue(IKeyInCache value, TimeSpan expiryTime);

    /// <summary>
    /// Добавляет значение
    /// </summary>
    /// <param name="value">Значение</param>
    /// <returns></returns>
    Task<bool> SetValue(IKeyInCache value);

    /// <summary>
    /// Удаляет значение
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <returns></returns>
    Task<bool> RemoveValue(string key);

    /// <summary>
    /// Проверяет наличие ключа
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <returns></returns>
    Task<bool> KeyExists(string key);
}