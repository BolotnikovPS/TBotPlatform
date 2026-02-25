using TBotPlatform.Extension;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsIsNullIsNotNullTests
{
    [Test]
    public void IsNotNull_NonNullObject_ReturnsTrue() => Assert.That(new object().IsNotNull(), Is.True);

    [Test]
    public void IsNotNull_NullObject_ReturnsFalse() => Assert.That(((object?)default).IsNotNull(), Is.False);

    [Test]
    public void IsNull_Object_ReturnsOppositeOfIsNotNull()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(new object().IsNull(), Is.False);
            Assert.That(((object?)default).IsNull(), Is.True);
        };
    }

    [TestCase("", true)]
    [TestCase("   ", true)]
    [TestCase("a", false)]
    public void IsNull_String_ChecksNullOrWhiteSpace(string value, bool expected) => Assert.That(value.IsNull(), Is.EqualTo(expected));

    [Test]
    public void IsNull_String_WhenNull_ReturnsTrue()
    {
        string? value = null;
        Assert.That(value.IsNull(), Is.True);
    }

    [Test]
    public void IsNotDefault_Struct_WhenNotDefault_ReturnsTrue()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(42.IsNotDefault(), Is.True);
            Assert.That(DateTime.MinValue.AddDays(1).IsNotDefault(), Is.True);
        };
    }

    [Test]
    public void IsNotDefault_Struct_WhenDefault_ReturnsFalse()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(0.IsNotDefault(), Is.False);
            Assert.That(default(DateTime).IsNotDefault(), Is.False);
        };
    }

    [Test]
    public void IsDefault_Struct_ReturnsOppositeOfIsNotDefault()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(0.IsDefault(), Is.True);
            Assert.That(1.IsDefault(), Is.False);
        };
    }

    [Test]
    public void IsNull_List_WhenEmptyOrNull_ReturnsTrue()
    {
        List<object>? nullList = null;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(nullList!.IsNull(), Is.True);
            Assert.That(new List<object>().IsNull(), Is.True);
        }
    }

    [Test]
    public void IsNull_List_WhenHasItems_ReturnsFalse()
    {
        var list = new List<object> { new() };
        Assert.That(list.IsNull(), Is.False);
    }

    [Test]
    public void IsNull_IEnumerable_WhenEmpty_ReturnsTrue() => Assert.That(Enumerable.Empty<object>().IsNull(), Is.True);

    [Test]
    public void IsNull_IQueryable_WhenEmpty_ReturnsTrue() => Assert.That(Enumerable.Empty<object>().AsQueryable().IsNull(), Is.True);
}
