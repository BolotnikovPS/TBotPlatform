using Telegram.Bot.Extensions;
using Telegram.Bot.Types;

namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Escapes HTML special characters (&amp;, &lt;, &gt;, &quot;) in text.
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Text safe to send with ParseMode = Html</returns>
    public static string? ToEscapeHtml(this string? text) => HtmlText.Escape(text);

    /// <summary>
    /// Converts message text honoring Telegram formatting into HTML markup.
    /// </summary>
    /// <param name="message">Message</param>
    /// <returns>Text with HTML markup or null if the message has no text or caption</returns>
    public static string? ToTelegramHtml(this Message? message) => message?.ToHtml();

    /// <summary>
    /// Removes HTML markup from text.
    /// </summary>
    /// <param name="html">Text with HTML markup</param>
    /// <returns>Text without markup</returns>
    public static string? ToPlainText(this string? html) => string.IsNullOrEmpty(html) ? html : HtmlText.ToPlain(html!);

    /// <summary>
    /// Returns the length of text excluding HTML markup.
    /// </summary>
    /// <param name="html">Text with HTML markup</param>
    /// <returns>Number of characters excluding tags and HTML entities</returns>
    public static int ToPlainLength(this string? html) => string.IsNullOrEmpty(html) ? 0 : HtmlText.PlainLength(html!);
}
