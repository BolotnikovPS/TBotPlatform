using Newtonsoft.Json;

namespace TBotPlatform.Extension;

public static partial class Extensions
{
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };

    public static string ToJson(this object obj, Formatting formatting)
        => obj != null
            ? JsonConvert.SerializeObject(obj, formatting, JsonSettings)
            : null;

    public static string ToJson<T>(this T obj)
        where T : class
        => obj?.ToJson(Formatting.None);

    public static string ToJson(this object obj)
        => obj?.ToJson(Formatting.None);

    /// <summary>
    /// Делает текст жирным
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public static string ToBoldString(this string expr) => $"<b>{expr}</b>";

    public static IEnumerable<string> SplitByLength(this string str, int maxLength)
    {
        for (var index = 0; index < str.Length; index += maxLength)
        {
            yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
        }
    }
}