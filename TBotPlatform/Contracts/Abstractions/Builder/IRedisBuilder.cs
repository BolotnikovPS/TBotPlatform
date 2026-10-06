namespace TBotPlatform.Contracts.Abstractions.Builder;

public interface IRedisBuilder
{
    /// <summary>
    /// Adds a prefix for Redis keys
    /// </summary>
    /// <param name="prefix">Prefix of keys in the cache</param>
    IRedisBuilder AddPrefix(string prefix);

    /// <summary>
    /// Adds Redis health check tags
    /// </summary>
    /// <param name="tags">Health check tags</param>
    IRedisBuilder AddHealthTags(string[] tags);

    /// <summary>
    /// Adds a health check name
    /// </summary>
    /// <param name="healthName">Health check name</param>
    IRedisBuilder AddHealthName(string healthName);

    /// <summary>
    /// Builds the Redis cache
    /// </summary>
    ICacheBuilder Build();
}
