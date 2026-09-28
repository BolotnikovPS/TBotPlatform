using System.Security;
using Telegram.Bot;

namespace TBotPlatform.Extension;

public static partial class Extensions
{
    private const string AuthDateField = "auth_date";

    /// <summary>
    /// Проверяет и разбирает данные Telegram.WebApp.initData (или LoginWidget)
    /// </summary>
    /// <param name="initData">Данные вида query string, полученные от Telegram</param>
    /// <param name="botToken">Токен бота</param>
    /// <param name="fields">Поля данных без хэша при успешной проверке, иначе null</param>
    /// <param name="maxAge">Максимальный возраст данных, если требуется защита от повторного использования</param>
    /// <returns>true, если подпись корректна и данные не устарели</returns>
    public static bool TryValidateInitData(
        this string? initData,
        string botToken,
        out SortedDictionary<string, string>? fields,
        TimeSpan? maxAge = null
        )
    {
        fields = null;

        if (string.IsNullOrWhiteSpace(initData) || string.IsNullOrWhiteSpace(botToken))
        {
            return false;
        }

        try
        {
            fields = AuthHelpers.ParseValidateData(initData, botToken);
        }
        catch (SecurityException)
        {
            return false;
        }

        if (maxAge is null)
        {
            return true;
        }

        if (!fields.TryGetValue(AuthDateField, out var authDateRaw) || !long.TryParse(authDateRaw, out var authDateUnix))
        {
            fields = null;

            return false;
        }

        var authDate = DateTimeOffset.FromUnixTimeSeconds(authDateUnix);

        if (DateTimeOffset.UtcNow - authDate > maxAge.Value)
        {
            fields = null;

            return false;
        }

        return true;
    }
}
