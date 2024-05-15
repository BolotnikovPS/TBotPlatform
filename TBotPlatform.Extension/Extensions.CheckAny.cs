namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Проверяет структуры на != null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool CheckAny<T>(this T obj)
        where T : struct
        => !obj.Equals(default(T));

    /// <summary>
    /// Проверяет объект на != null
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool CheckAny(this object obj)
        => obj != null;

    /// <summary>
    /// Проверяет объект на != null
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool CheckAny(this string s)
        => !string.IsNullOrEmpty(s);

    /// <summary>
    /// Проверяет List на наличие данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool CheckAny<T>(this List<T> collection)
        where T : class
        => collection?.Count > 0;

    /// <summary>
    /// Проверяет IEnumerable на наличие данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool CheckAny<T>(this IEnumerable<T> collection)
        where T : class
        => collection?.Any() == true;

    /// <summary>
    /// Проверяет IQueryable на наличие данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool CheckAny<T>(this IQueryable<T> collection)
        where T : class
        => collection?.Any() == true;
}