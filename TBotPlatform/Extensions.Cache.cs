using TBotPlatform.Contracts.Abstractions.Cache;
using TBotPlatform.Results;
using TBotPlatform.Results.Abstractions;
using ZiggyCreatures.Caching.Fusion;

namespace TBotPlatform;

public static partial class Extensions
{
    private const string DataNotFound = "Данных не найдено.";
    private const string CollectionTagPrefix = "collection:";

    public static async Task<IResult> AddValueToCollection(
        this IFusionCache fusionCache,
        string collection,
        IKeyInCache value,
        CancellationToken cancellationToken = default
        )
    {
        try
        {
            var fullKey = CreateCollectionKey(collection, value.Key);
            var tag = CreateCollectionTag(collection);

            // Добавляем значение с тегом коллекции (FusionCache сам сериализует через NewtonsoftJson)
            await fusionCache.SetAsync(fullKey, value, tags: [tag], token: cancellationToken);

            return Result.Success();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return Result.Failure(ErrorResult.Failure(""));
        }
    }

    public static async Task<IResult<T>> GetValueFromCollection<T>(
        this IFusionCache fusionCache,
        string collection,
        string key,
        CancellationToken cancellationToken = default
        )
        where T : IKeyInCache
    {
        try
        {
            var fullKey = CreateCollectionKey(collection, key);
            var value = await fusionCache.TryGetAsync<T>(fullKey, token: cancellationToken);

            return value.HasValue
                ? ResultT<T>.Success(value.Value)
                : ResultT<T>.Failure(ErrorResult.NotFound(DataNotFound));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return ResultT<T>.Failure(ErrorResult.Failure(""));
        }
    }

    public static async Task<IResult> RemoveValueFromCollection(
        this IFusionCache fusionCache,
        string collection,
        string key,
        CancellationToken cancellationToken = default
        )
    {
        try
        {
            var fullKey = CreateCollectionKey(collection, key);

            await fusionCache.RemoveAsync(fullKey, token: cancellationToken);

            return Result.Success();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return Result.Failure(ErrorResult.NotFound(DataNotFound));
        }
    }

    public static async Task<IResult> RemoveCollection(
        this IFusionCache fusionCache,
        string collection,
        CancellationToken cancellationToken = default
        )
    {
        try
        {
            var tag = CreateCollectionTag(collection);

            await fusionCache.RemoveByTagAsync(tag, token: cancellationToken);

            return Result.Success();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return Result.Failure(ErrorResult.NotFound(DataNotFound));
        }
    }

    public static async Task<IResult<T>> GetValue<T>(
        this IFusionCache fusionCache,
        string key,
        CancellationToken cancellationToken = default
        )
        where T : IKeyInCache
    {
        var value = await fusionCache.TryGetAsync<T>(key, token: cancellationToken);

        return value.HasValue
            ? ResultT<T>.Success(value.Value)
            : ResultT<T>.Failure(ErrorResult.NotFound(DataNotFound));
    }

    public static async Task<IResult> SetValue(
        this IFusionCache fusionCache,
        IKeyInCache value,
        TimeSpan expiryTime,
        CancellationToken cancellationToken = default
        )
    {
        try
        {
            await fusionCache.SetAsync(value.Key, value, expiryTime, token: cancellationToken);

            return Result.Success();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return Result.Failure(ErrorResult.Failure(""));
        }
    }

    public static async Task<IResult> SetValue(
        this IFusionCache fusionCache,
        IKeyInCache value,
        CancellationToken cancellationToken = default
        )
    {
        try
        {
            await fusionCache.SetAsync(value.Key, value, token: cancellationToken);

            return Result.Success();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return Result.Failure(ErrorResult.Failure(""));
        }
    }

    public static async Task<IResult> RemoveValue(
        this IFusionCache fusionCache,
        string key,
        CancellationToken cancellationToken = default
        )
    {
        try
        {
            await fusionCache.RemoveAsync(key, token: cancellationToken);

            return Result.Success();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return Result.Failure(ErrorResult.Failure(""));
        }
    }

    public static async Task<IResult<bool>> KeyExists(
        this IFusionCache fusionCache,
        string key,
        CancellationToken cancellationToken = default
        )
    {
        try
        {
            // Проверяем факт наличия ключа: тип значения заранее неизвестен,
            // поэтому читаем его как object (иначе для нестроковых значений будет ошибка приведения типа).
            var exists = await fusionCache.TryGetAsync<object>(key, token: cancellationToken);

            return ResultT<bool>.Success(exists.HasValue);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return ResultT<bool>.Failure(ErrorResult.Failure(""));
        }
    }

    private static string CreateCollectionKey(string collection, string key) => $"{collection}:{key}";

    private static string CreateCollectionTag(string collection) => $"{CollectionTagPrefix}{collection}";
}
