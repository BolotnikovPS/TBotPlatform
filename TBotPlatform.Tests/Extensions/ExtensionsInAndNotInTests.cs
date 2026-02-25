using TBotPlatform.Extension;
using TBotPlatform.Results.Enums;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsInAndNotInTests
{
    [Test]
    public void In_String_WhenContained_IgnoresCase_ReturnsTrue()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That("AB".In("ab", "cd"), Is.True);
            Assert.That("ab".In("AB", "CD"), Is.True);
        };
    }

    [Test]
    public void In_String_WhenNotContained_ReturnsFalse() => Assert.That("x".In("a", "b"), Is.False);

    [Test]
    public void In_String_WhenInputNullOrEmpty_ReturnsFalse()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(((string)null!).In("a"), Is.False);
            Assert.That("".In("a"), Is.False);
        };
    }

    [Test]
    public void NotIn_String_ReturnsOppositeOfIn()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That("ab".NotIn("ab", "cd"), Is.False);
            Assert.That("x".NotIn("a", "b"), Is.True);
        };
    }

    [Test]
    public void In_Int_WhenContained_ReturnsTrue() => Assert.That(2.In(1, 2, 3), Is.True);

    [Test]
    public void In_Int_WhenNotContained_ReturnsFalse() => Assert.That(5.In(1, 2, 3), Is.False);

    [Test]
    public void In_Int_EmptyArray_ReturnsFalse() => Assert.That(1.In([]), Is.False);

    [Test]
    public void NotIn_Int_ReturnsOppositeOfIn()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(2.NotIn(1, 3), Is.True);
            Assert.That(2.NotIn(1, 2, 3), Is.False);
        };
    }

    [Test]
    public void In_Long_WhenContained_ReturnsTrue() => Assert.That(2L.In(1L, 2L, 3L), Is.True);

    [Test]
    public void NotIn_Long_ReturnsOppositeOfIn() => Assert.That(5L.NotIn(1L, 2L), Is.True);

    [Test]
    public void In_Enum_WhenContained_ReturnsTrue() => Assert.That(ErrorResultType.Failure.In(ErrorResultType.Failure, ErrorResultType.NotFound), Is.True);

    [Test]
    public void In_Enum_WhenNotContained_ReturnsFalse() => Assert.That(ErrorResultType.Conflict.In(ErrorResultType.Failure, ErrorResultType.NotFound), Is.False);

    [Test]
    public void NotIn_Enum_ReturnsOppositeOfIn() => Assert.That(ErrorResultType.Validation.NotIn(ErrorResultType.Failure, ErrorResultType.NotFound), Is.True);
}
