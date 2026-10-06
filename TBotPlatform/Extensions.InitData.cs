using System.Security;
using Telegram.Bot;

namespace TBotPlatform.Extension;

public static partial class Extensions
{
    private const string AuthDateField = "auth_date";

    /// <summary>
    /// Checks and parses Telegram.WebApp.initData (or LoginWidget) data
    /// </summary>
    /// <param name="initData">Data in query string form received from Telegram</param>
    /// <param name="botToken">Bot token</param>
    /// <param name="fields">Data fields without the hash on successful validation, otherwise null</param>
    /// <param name="maxAge">Maximum data age, if protection against reuse is required</param>
    /// <returns>true if the signature is correct and the data is not stale</returns>
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
