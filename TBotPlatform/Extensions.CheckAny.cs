namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Checks that the string contains non-whitespace data
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool CheckAny(this string? s)
        => !string.IsNullOrWhiteSpace(s);

    /// <summary>
    /// Checks that the list has items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool CheckAny<T>(this List<T>? collection)
        where T : class
        => collection?.Count > 0;

    /// <summary>
    /// Checks that the enumerable has items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool CheckAny<T>(this IEnumerable<T>? collection)
        where T : class
        => collection?.Any() == true;

    /// <summary>
    /// Checks that the queryable has items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool CheckAny<T>(this IQueryable<T>? collection)
        where T : class
        => collection?.Any() == true;
}