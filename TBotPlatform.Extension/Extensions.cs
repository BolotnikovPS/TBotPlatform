using Newtonsoft.Json;

namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Делает текст жирным
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public static string ToBoldString(this string expr) => $"<b>{expr}</b>";

    public static string ToJson<T>(this T obj)
        where T : class => JsonConvert.SerializeObject(obj);

    public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj);
    
    public static IEnumerable<string> SplitByLength(this string str, int maxLength)
    {
        for (var index = 0; index < str.Length; index += maxLength)
        {
            yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
        }
    }
}