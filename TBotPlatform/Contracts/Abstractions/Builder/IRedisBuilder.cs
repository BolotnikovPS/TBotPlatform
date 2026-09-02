namespace TBotPlatform.Contracts.Abstractions.Builder;

public interface IRedisBuilder
{
    /// <summary>
    /// Добавляет префикс для ключей redis
    /// </summary>
    /// <param name="prefix">Префикс ключей в кеше</param>
    /// <returns></returns>
    IRedisBuilder AddPrefix(string prefix);

    /// <summary>
    /// Добавляет теги хелсчека redis
    /// </summary>
    /// <param name="tags">Теги хелсчека</param>
    /// <returns></returns>
    IRedisBuilder AddHealthTags(string[] tags);

    /// <summary>
    /// Добавляет название хелсчека
    /// </summary>
    /// <param name="healthName">Название хелсчека</param>
    /// <returns></returns>
    IRedisBuilder AddHealthName(string healthName);

    /// <summary>
    /// Собирает redis кеш
    /// </summary>
    /// <returns></returns>
    ICacheBuilder Build();
}
