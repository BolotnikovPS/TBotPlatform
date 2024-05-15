namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Проверяет структуры на default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsDefault<T>(this T obj)
        where T : struct
        => !obj.IsNotDefault();

    /// <summary>
    /// Проверяет объект на null
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNull(this object obj)
        => !obj.IsNotNull();

    /// <summary>
    /// Проверяет объект на null
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsNull(this string s)
        => !s.CheckAny();

    /// <summary>
    /// Проверяет List на отсутствие данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool IsNull<T>(this List<T> collection)
        where T : class
        => !collection.CheckAny();

    /// <summary>
    /// Проверяет IEnumerable на отсутствие данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool IsNull<T>(this IEnumerable<T> collection)
        where T : class
        => !collection.CheckAny();

    /// <summary>
    /// Проверяет IQueryable на отсутствие данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool IsNull<T>(this IQueryable<T> collection)
        where T : class
        => !collection.CheckAny();
}