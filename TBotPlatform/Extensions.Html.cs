using Telegram.Bot.Extensions;
using Telegram.Bot.Types;

namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Экранирует специальные символы HTML (&amp;, &lt;, &gt;, &quot;) в тексте
    /// </summary>
    /// <param name="text">Текст</param>
    /// <returns>Текст, безопасный для отправки с ParseMode = Html</returns>
    public static string? ToEscapeHtml(this string? text) => HtmlText.Escape(text);

    /// <summary>
    /// Преобразует текст сообщения с учетом форматирования Telegram в HTML-разметку
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <returns>Текст с HTML-разметкой или null, если в сообщении нет текста и подписи</returns>
    public static string? ToTelegramHtml(this Message? message) => message?.ToHtml();

    /// <summary>
    /// Убирает HTML-разметку из текста
    /// </summary>
    /// <param name="html">Текст с HTML-разметкой</param>
    /// <returns>Текст без разметки</returns>
    public static string? ToPlainText(this string? html) => string.IsNullOrEmpty(html) ? html : HtmlText.ToPlain(html!);

    /// <summary>
    /// Возвращает длину текста без учета HTML-разметки
    /// </summary>
    /// <param name="html">Текст с HTML-разметкой</param>
    /// <returns>Количество символов без учета тегов и HTML-сущностей</returns>
    public static int ToPlainLength(this string? html) => string.IsNullOrEmpty(html) ? 0 : HtmlText.PlainLength(html!);
}
