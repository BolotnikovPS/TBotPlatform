namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Проверяет структуры на != default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNotDefault<T>(this T obj)
        where T : struct
        => !obj.Equals(default(T));

    /// <summary>
    /// Проверяет объект на != null
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNotNull(this object obj)
        => obj != null;
}