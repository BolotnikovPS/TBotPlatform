using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using TBotPlatform.Extension;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsInitDataTests
{
    private const string BotToken = "123456:AAbbCCddEEffGGhhIIjjKKllMMnnOOppQQr";
    private const string QueryId = "AAF";

    [Test]
    public void TryValidateInitData_WhenHashIsValid_ReturnsFieldsWithoutHash()
    {
        var initData = BuildInitData(DateTimeOffset.UtcNow, BotToken);

        var result = initData.TryValidateInitData(BotToken, out var fields);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(fields, Is.Not.Null);
            Assert.That(fields!.ContainsKey("hash"), Is.False);
            Assert.That(fields["query_id"], Is.EqualTo(QueryId));
        }
    }

    [Test]
    public void TryValidateInitData_WhenHashIsInvalid_ReturnsFalse()
    {
        var initData = BuildInitData(DateTimeOffset.UtcNow, BotToken, new string('0', 64));

        var result = initData.TryValidateInitData(BotToken, out var fields);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
            Assert.That(fields, Is.Null);
        }
    }

    [Test]
    public void TryValidateInitData_WhenHashIsValidButAnotherToken_ReturnsFalse()
    {
        var initData = BuildInitData(DateTimeOffset.UtcNow, BotToken);

        var result = initData.TryValidateInitData("654321:ZZbbCCddEEffGGhhIIjjKKllMMnnOOppQQr", out var fields);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
            Assert.That(fields, Is.Null);
        }
    }

    [Test]
    public void TryValidateInitData_WhenInitDataIsEmpty_ReturnsFalse()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(((string?)null).TryValidateInitData(BotToken, out var nullFields), Is.False);
            Assert.That(nullFields, Is.Null);
            Assert.That(string.Empty.TryValidateInitData(BotToken, out var emptyFields), Is.False);
            Assert.That(emptyFields, Is.Null);
            Assert.That("query_id=AAF".TryValidateInitData(" ", out var blankTokenFields), Is.False);
            Assert.That(blankTokenFields, Is.Null);
        }
    }

    [Test]
    public void TryValidateInitData_WhenMaxAgeIsNotExpired_ReturnsTrue()
    {
        var initData = BuildInitData(DateTimeOffset.UtcNow.AddMinutes(-5), BotToken);

        var result = initData.TryValidateInitData(BotToken, out var fields, TimeSpan.FromHours(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(fields, Is.Not.Null);
        }
    }

    [Test]
    public void TryValidateInitData_WhenMaxAgeIsExpired_ReturnsFalse()
    {
        var initData = BuildInitData(DateTimeOffset.UtcNow.AddHours(-2), BotToken);

        var result = initData.TryValidateInitData(BotToken, out var fields, TimeSpan.FromHours(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
            Assert.That(fields, Is.Null);
        }
    }

    [Test]
    public void TryValidateInitData_WhenAuthDateIsMissingAndMaxAgeIsSet_ReturnsFalse()
    {
        var initData = BuildInitData(DateTimeOffset.UtcNow, BotToken, includeAuthDate: false);

        var result = initData.TryValidateInitData(BotToken, out var fields, TimeSpan.FromHours(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
            Assert.That(fields, Is.Null);
        }
    }

    private static string BuildInitData(
        DateTimeOffset authDate,
        string botToken,
        string? hashOverride = null,
        bool includeAuthDate = true
        )
    {
        var fields = new SortedDictionary<string, string>
        {
            ["query_id"] = QueryId,
            ["user"] = "{\"id\":1,\"first_name\":\"user\"}",
        };

        if (includeAuthDate)
        {
            fields["auth_date"] = authDate.ToUnixTimeSeconds().ToString();
        }

        var dataCheckString = string.Join('\n', fields.Select(x => $"{x.Key}={x.Value}"));
        var secretKey = HMACSHA256.HashData(Encoding.ASCII.GetBytes("WebAppData"), Encoding.ASCII.GetBytes(botToken));
        var hash = hashOverride ?? Convert.ToHexString(HMACSHA256.HashData(secretKey, Encoding.UTF8.GetBytes(dataCheckString)));

        var query = string.Join('&', fields.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        return $"{query}&hash={hash}";
    }
}
