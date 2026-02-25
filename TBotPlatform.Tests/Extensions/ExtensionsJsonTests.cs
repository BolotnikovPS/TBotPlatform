using TBotPlatform.Extension;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsJsonTests
{
    [Test]
    public void ToJson_NonNullObject_ReturnsJsonString()
    {
        var obj = new { A = 1, B = "x" };
        var json = obj.ToJson();
        Assert.That(json, Does.Contain("\"A\":1"));
        Assert.That(json, Does.Contain("\"B\":\"x\""));
    }

    [Test]
    public void ToJson_NullObject_ReturnsNull()
    {
        object? obj = null;
        Assert.That(obj!.ToJson(), Is.Null);
    }

    [Test]
    public void FromJson_ValidJson_Deserializes()
    {
        var json = "{\"A\":1,\"B\":\"x\"}";
        var result = json.FromJson<JsonTestDto>();
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.A, Is.EqualTo(1));
            Assert.That(result.B, Is.EqualTo("x"));
        }
    }

    [Test]
    public void FromJson_NullOrEmpty_ReturnsDefault() => Assert.That(((string?)default).FromJson<JsonTestDto>(), Is.Null);

    [Test]
    public void TryParseJson_ValidJson_ReturnsTrueAndResult()
    {
        var json = "{\"A\":10,\"B\":\"y\"}";
        var ok = json.TryParseJson<JsonTestDto>(out var result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(result, Is.Not.Null);
        }
        Assert.That(result!.A, Is.EqualTo(10));
    }

    [Test]
    public void TryParseJson_InvalidJson_ReturnsFalseAndDefault()
    {
        var ok = "not json".TryParseJson<JsonTestDto>(out var result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.False);
            Assert.That(result, Is.Null);
        }
    }

    [Test]
    public void TryParseJson_Null_ReturnsFalseAndDefault()
    {
        var ok = ((string?)default).TryParseJson<JsonTestDto>(out var result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.False);
            Assert.That(result, Is.Null);
        }
    }

    private class JsonTestDto
    {
        public int A { get; set; }
        public string? B { get; set; }
    }
}
