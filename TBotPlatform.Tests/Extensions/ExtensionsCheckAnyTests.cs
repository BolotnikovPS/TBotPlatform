using TBotPlatform.Extension;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsCheckAnyTests
{
    [TestCase("", false)]
    [TestCase("   ", false)]
    [TestCase("a", true)]
    [TestCase("text", true)]
    public void CheckAny_String_ReturnsExpected(string value, bool expected) => Assert.That(value.CheckAny(), Is.EqualTo(expected));

    [Test]
    public void CheckAny_String_WhenNull_ReturnsFalse()
    {
        string? value = null;
        Assert.That(value.CheckAny(), Is.False);
    }

    [Test]
    public void CheckAny_ListWithItems_ReturnsTrue()
    {
        var list = new List<object> { new() };
        Assert.That(list.CheckAny(), Is.True);
    }

    [Test]
    public void CheckAny_EmptyList_ReturnsFalse()
    {
        var list = new List<object>();
        Assert.That(list.CheckAny(), Is.False);
    }

    [Test]
    public void CheckAny_NullList_ReturnsFalse()
    {
        List<object> list = null!;
        Assert.That(list.CheckAny(), Is.False);
    }

    [Test]
    public void CheckAny_IEnumerableWithItems_ReturnsTrue()
    {
        var collection = new[] { new object() };
        Assert.That(collection.CheckAny(), Is.True);
    }

    [Test]
    public void CheckAny_EmptyIEnumerable_ReturnsFalse()
    {
        var collection = Enumerable.Empty<object>();
        Assert.That(collection.CheckAny(), Is.False);
    }

    [Test]
    public void CheckAny_IQueryableWithItems_ReturnsTrue()
    {
        var query = new[] { new object() }.AsQueryable();
        Assert.That(query.CheckAny(), Is.True);
    }

    [Test]
    public void CheckAny_EmptyIQueryable_ReturnsFalse()
    {
        var query = Enumerable.Empty<object>().AsQueryable();
        Assert.That(query.CheckAny(), Is.False);
    }
}
