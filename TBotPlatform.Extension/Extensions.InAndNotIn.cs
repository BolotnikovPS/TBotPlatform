namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Проверяет наличие input в value без регистровой зависимости
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    /// <returns>bool</returns>
    public static bool In(this string input, params string[] value)
    {
        if (string.IsNullOrEmpty(input)
            || value?.Length == 0
           )
        {
            return false;
        }

        return value
              .Where(x => x.CheckAny())
              .Select(z => z.ToUpper())
              .Contains(input.ToUpper());
    }

    /// <summary>
    /// Проверяет отсутствие input в value без регистровой зависимости
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    /// <returns>bool</returns>
    public static bool NotIn(this string input, params string[] value) => !input.In(value);

    /// <summary>
    /// Проверяет наличие input в value без регистровой зависимости
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    /// <returns>bool</returns>
    public static bool In<T>(this string input, params T[] value)
        where T : Enum
    {
        if (string.IsNullOrEmpty(input)
            || value?.Length == 0
           )
        {
            return false;
        }

        return value
              .Where(x => x.IsNotNull())
              .Select(
                   z => z
                       .ToString()
                       .ToUpper()
                   )
              .Contains(input.ToUpper());
    }

    /// <summary>
    /// Проверяет отсутствие input в value без регистровой зависимости
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    /// <returns>bool</returns>
    public static bool NotIn<T>(this string input, params T[] value)
        where T : Enum => !input.In(value);

    /// <summary>
    /// Проверяет наличие input в value без регистровой зависимости
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    /// <returns>bool</returns>
    public static bool In(this int input, params int[] value)
    {
        return value.Length != 0 && value.Any(z => z == input);
    }

    /// <summary>
    /// Проверяет отсутствие input в value без регистровой зависимости
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    /// <returns>bool</returns>
    public static bool NotIn(this int input, params int[] value) => !input.In(value);

    /// <summary>
    /// Проверяет наличие input в value без регистровой зависимости
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    /// <returns>bool</returns>
    public static bool In<T>(this T input, params T[] value)
        where T : Enum
    {
        if (input.IsNull()
            || value?.Length == 0)
        {
            return false;
        }

        return value.Contains(input);
    }

    /// <summary>
    /// Проверяет отсутствие input в value без регистровой зависимости
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    /// <returns>bool</returns>
    public static bool NotIn<T>(this T input, params T[] value)
        where T : Enum => !input.In(value);
}