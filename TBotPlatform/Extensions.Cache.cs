using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform;

public static partial class Extensions
{
    private const string DataNotFound = "Данных не найдено.";
    private const string CollectionTagPrefix = "collection:";

    public static async Task<IResult> AddValueToCollection(this IFusionCache fusionCache, string collection, IKeyInCache value)
    {
        try
        {
            var fullKey = CreateCollectionKey(collection, value.Key);
            var tag = CreateCollectionTag(collection);

            // Добавляем значение с тегом коллекции (FusionCache сам сериализует через NewtonsoftJson)
            await fusionCache.SetAsync(fullKey, value, tags: [tag]);

            return Result.Success();
        }
        catch
        {
            return Result.Failure(ErrorResult.Failure(""));
        }
    }

    public static async Task<IResult<T>> GetValueFromCollection<T>(this IFusionCache fusionCache, string collection, string key)
        where T : IKeyInCache
    {
        try
        {
            var fullKey = CreateCollectionKey(collection, key);
            var value = await fusionCache.TryGetAsync<T>(fullKey);

            return value.HasValue
                ? ResultT<T>.Success(value.Value)
                : ResultT<T>.Failure(ErrorResult.NotFound(DataNotFound));
        }
        catch
        {
            return ResultT<T>.Failure(ErrorResult.Failure(""));
        }
    }

    public static async Task<IResult> RemoveValueFromCollection(this IFusionCache fusionCache, string collection, string key)
    {
        try
        {
            var fullKey = CreateCollectionKey(collection, key);

            await fusionCache.RemoveAsync(fullKey);

            return Result.Success();
        }
        catch
        {
            return Result.Failure(ErrorResult.NotFound(DataNotFound));
        }
    }

    public static async Task<IResult> RemoveCollection(this IFusionCache fusionCache, string collection)
    {
        try
        {
            var tag = CreateCollectionTag(collection);

            await fusionCache.RemoveByTagAsync(tag);

            return Result.Success();
        }
        catch
        {
            return Result.Failure(ErrorResult.NotFound(DataNotFound));
        }
    }

    public static async Task<IResult<T>> GetValue<T>(this IFusionCache fusionCache, string key)
        where T : IKeyInCache
    {
        var value = await fusionCache.TryGetAsync<T>(key);

        return value.HasValue
            ? ResultT<T>.Success(value.Value)
            : ResultT<T>.Failure(ErrorResult.NotFound(DataNotFound));
    }

    public static async Task<IResult> SetValue(this IFusionCache fusionCache, IKeyInCache value, TimeSpan expiryTime)
    {
        try
        {
            await fusionCache.SetAsync(value.Key, value, expiryTime);

            return Result.Success();
        }
        catch
        {
            return Result.Failure(ErrorResult.Failure(""));
        }
    }

    public static async Task<IResult> SetValue(this IFusionCache fusionCache, IKeyInCache value)
    {
        try
        {
            await fusionCache.SetAsync(value.Key, value);

            return Result.Success();
        }
        catch
        {
            return Result.Failure(ErrorResult.Failure(""));
        }
    }

    public static async Task<IResult> RemoveValue(this IFusionCache fusionCache, string key)
    {
        try
        {
            await fusionCache.RemoveAsync(key);

            return Result.Success();
        }
        catch
        {
            return Result.Failure(ErrorResult.Failure(""));
        }
    }

    public static async Task<IResult<bool>> KeyExists(this IFusionCache fusionCache, string key)
    {
        try
        {
            var exists = await fusionCache.TryGetAsync<string>(key);

            return ResultT<bool>.Success(exists.HasValue);
        }
        catch
        {
            return ResultT<bool>.Failure(ErrorResult.Failure(""));
        }
    }

    private static string CreateCollectionKey(string collection, string key) => $"{collection}:{key}";

    private static string CreateCollectionTag(string collection) => $"{CollectionTagPrefix}{collection}";
}
