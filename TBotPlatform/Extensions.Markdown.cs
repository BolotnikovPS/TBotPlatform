namespace TBotPlatform.Extension;

public static partial class Extensions
{
    public static string ToBoldString(this string expr) => $"<b>{expr}</b>";

    public static string ToItalicString(this string expr) => $"<i>{expr}</i>";

    public static string ToStrikethroughString(this string expr) => $"<s>{expr}</s>";

    public static string ToUnderlineString(this string expr) => $"<u>{expr}</u>";

    public static string ToSpoilerString(this string expr) => $"<tg-spoiler>{expr}</tg-spoiler>";

    public static string ToCodeString(this string expr) => $"<code>{expr}</code>";

    public static string ToPreString(this string expr) => $"<pre>{expr}</pre>";
}