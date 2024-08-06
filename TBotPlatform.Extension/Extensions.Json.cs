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
        => obj.IsNotNull()
            ? JsonConvert.SerializeObject(obj, formatting, JsonSettings)
            : null;

    public static string ToJson<T>(this T obj)
        where T : class
        => obj?.ToJson(Formatting.None);

    public static string ToJson(this object obj)
        => obj?.ToJson(Formatting.None);

    public static T FromJson<T>(this string value)
        => value.IsNotNull()
            ? JsonConvert.DeserializeObject<T>(value, JsonSettings)
            : default;

    public static bool TryParseJson<T>(this string value, out T result)
    {
        if (value.IsNull())
        {
            result = default;
            return false;
        }

        try
        {
            result = value.FromJson<T>();
            return true;
        }
        catch
        {
            result = default;
        }

        return false;
    }
}