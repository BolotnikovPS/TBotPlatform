namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Checks structures for != default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNotDefault<T>(this T obj)
        where T : struct
        => !obj.Equals(default(T));

    /// <summary>
    /// Checks an object for != null
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNotNull(this object? obj)
        => obj != null;
}