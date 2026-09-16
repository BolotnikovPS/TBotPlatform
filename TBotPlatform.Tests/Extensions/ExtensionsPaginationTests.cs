using NUnit.Framework;
using TBotPlatform;
using TBotPlatform.Contracts.Bots;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsPaginationTests
{
    [TestCase("$pag_1", 1)]
    [TestCase("$pag_42", 42)]
    public void TryParsePagination_ValidData_ReturnsTrueAndValue(string data, int expected)
    {
        var ok = data.TryParsePagination(out var result);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(result, Is.EqualTo(expected));
        }
    }

    [TestCase("hello")]
    [TestCase("")]
    public void TryParsePagination_InvalidData_ReturnsFalse(string data)
        => Assert.That(data.TryParsePagination(out _), Is.False);

    [Test]
    public void TryParsePagination_MarkupNextState_ReturnsTrueAndValue()
    {
        var ok = new MarkupNextState("State", "$pag_7").TryParsePagination(out var result);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(result, Is.EqualTo(7));
        }
    }

    [Test]
    public void TryParsePagination_MarkupNextState_NullData_ReturnsFalse()
        => Assert.That(new MarkupNextState("State", null).TryParsePagination(out _), Is.False);

    [Test]
    public void GetPaginationData_FirstPage_ReturnsFirstItemAndPagingFlags()
    {
        var result = new List<string> { "a", "b", "c" }.GetPaginationData(step: 1, currentPosition: 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Values, Is.EqualTo(new[] { "a" }));
            Assert.That(result.IsNext, Is.True);
            Assert.That(result.IsPrevious, Is.False);
        }
    }

    [Test]
    public void GetPaginationData_LastPage_ReturnsLastItemAndPreviousFlag()
    {
        var result = new List<string> { "a", "b", "c" }.GetPaginationData(step: 1, currentPosition: 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Values, Is.EqualTo(new[] { "c" }));
            Assert.That(result.IsNext, Is.False);
            Assert.That(result.IsPrevious, Is.True);
        }
    }
}
